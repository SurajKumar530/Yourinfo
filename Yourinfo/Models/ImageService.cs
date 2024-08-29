using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

public class ImageService
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public ImageService(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    private async Task<string> ProcessAndCompressImageAsync(IFormFile file, string folderPath, int width, int height, long maxSizeInBytes)
    {
        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, folderPath, uniqueFileName);

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        using (var image = Image.Load(file.OpenReadStream()))
        {
            image.Mutate(x => x.Resize(width, height));

            int quality = 100; // Start with the highest quality
            var jpegEncoder = new JpegEncoder { Quality = quality };

            // Save image to a temporary path first
            var tempFilePath = Path.GetTempFileName();
            await SaveImageAsync(image, tempFilePath, jpegEncoder);

            // Adjust quality until the file size is below the maxSizeInBytes
            while (new FileInfo(tempFilePath).Length > maxSizeInBytes)
            {
                quality -= 5; // Reduce quality by 5
                if (quality < 10) // Prevent quality from going too low
                {
                    break;
                }
                jpegEncoder = new JpegEncoder { Quality = quality };
                await SaveImageAsync(image, tempFilePath, jpegEncoder);
            }

            // Move the temp file to the final destination
            System.IO.File.Move(tempFilePath, filePath);

            return Path.Combine(folderPath, uniqueFileName).Replace("\\", "/"); // Return a relative URL
        }
    }

    private async Task SaveImageAsync(Image image, string filePath, JpegEncoder jpegEncoder)
    {
        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await image.SaveAsync(fileStream, jpegEncoder);
        }
    }

    public async Task<string> SaveImageAsync(IFormFile file, string folderPath, int width, int height)
    {
        // 2 MB in bytes
        const long maxSizeInBytes = 2 * 1024 * 1024;
        return await ProcessAndCompressImageAsync(file, folderPath, width, height, maxSizeInBytes);
    }
}

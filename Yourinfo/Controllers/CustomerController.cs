using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;
using Yourinfo.Models.Admin;
using Yourinfo.Models;
using Yourinfo.Models.Customer;
using Yourinfo.Repository.Interface;
using Utility;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using MailKit;

namespace Yourinfo.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        public readonly ICustomerRepository _customer;
        public readonly IUserRepository _user;
        public readonly IWebHostEnvironment _environment;
        private readonly ImageService imageService;
        public CustomerController(ICustomerRepository customer, IWebHostEnvironment environment, IUserRepository user, IDistributedCache distributedCache, ImageService imageService)
        {
            _customer = customer;
            _environment = environment;
            _user = user;
            this.imageService = imageService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    var username = user.Identity.Name;
                    var getCustomerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // You might need to get this from user claims or other sources

                    if (string.IsNullOrEmpty(getCustomerId))
                    {
                        return BadRequest("Invalid CustomerID.");
                    }

                    // Retrieve from the repository if not cached
                    CustomerDetails model = await _user.GetCustomerWithGalleryImages(getCustomerId);

                    var fullurl = model.FullURL;
                    var uri = new Uri(fullurl);
                    var path = uri.AbsolutePath; // "/TEST"

                    // Remove the leading slash
                    model.FullURL = path.TrimStart('/');

                    if (model == null)
                    {
                        return NotFound("Customer not found.");
                    }
                    return View(model);
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }

        }

        #region Gallery
        [HttpGet]
        public async Task<IActionResult> GetGalleryImages()
        {
            try
            {
                var user = HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    var getCustomerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    // Retrieve from the repository if not cached
                    List<CustomerGalleryImages> customerGalleryImages = _customer.getGalleryData(getCustomerId);
                    return View(customerGalleryImages);
                }

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> updateGalleryImage(IFormFile file, string customerId, string imagePath, string imageName)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return View("Upload Image");
                }
                CustomerGalleryImages images = new CustomerGalleryImages();
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

                // Check if the old file exists and delete it
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }


                images.CustomerGalleryPath = await imageService.SaveImageAsync(file, "uploads/galleryLogo", 350, 262);
                images.CustomerGalleryName = file.FileName;
                images.CustomerId = customerId;

                var result = await _customer.updateGalleryImages(images, imageName);
                if (result > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> deleteGalleryImage(string customerId, string imagePath, string imageName)
        {
            try
            {
                if (customerId == null)
                {
                    return View("Customer is not valid ");
                }

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

                // Check if the old file exists and delete it
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }


                var result = await _customer.deleteGalleryImages(customerId, imageName);
                if (result > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> addGalleryImage(List<IFormFile> files, string customerId)
        {
            try
            {
                if (files == null)
                {
                    return View("Upload Image");
                }
                List<GalleryImages> images = new List<GalleryImages>();

                // Handle Gallery images file uploads
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var gallery = new GalleryImages
                        {
                            ImagePath = await imageService.SaveImageAsync(file, "uploads/galleryLogo", 350, 262),
                            ImageName = file.FileName
                        };

                        images.Add(gallery);
                    }
                }


                var resultcode = await _user.SaveImagesToDatabaseAsync(images, Convert.ToInt32(customerId));
                if (resultcode == 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion Gallery

        #region Brocehure
        [HttpGet]
        public async Task<IActionResult> GetBrochure()
        {
            try
            {
                var user = HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    var getCustomerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    // Retrieve from the repository if not cached
                    List<CustomerBrochureImages> customerBrochureImages = _customer.getBrochureData(getCustomerId);

                    return View(customerBrochureImages);
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> updateBrochureImage(IFormFile file, string customerId, string imagePath, string imageName)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return View("Upload Image");
                }
                CustomerBrochureImages images = new CustomerBrochureImages();
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

                // Check if the old file exists and delete it
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                images.CustomerBrochurePath = await imageService.SaveImageAsync(file, "uploads/Brochure", 255, 255);
                images.CustomerBrochureName = file.FileName;
                images.CustomerId = customerId;
                
                var result = await _customer.updateBrochureImages(images, imageName);
                if (result > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> deleteBrochureImage(string customerId, string imagePath, string imageName)
        {
            try
            {
                if (customerId == null)
                {
                    return View("Customer is not valid ");
                }

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

                // Check if the old file exists and delete it
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                var result = await _customer.deleteBrochureImages(customerId, imageName);
                if (result > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> addBrochureImage(List<IFormFile> files, string customerId)
        {
            try
            {
                if (files == null)
                {
                    return View("Upload Image");
                }
                List<BrochureImages> images = new List<BrochureImages>();

                // Handle Gallery images file uploads
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var brochure = new BrochureImages
                        {
                            ImagePath = await imageService.SaveImageAsync(file, "uploads/Brochure", 255, 255),
                            ImageName = file.FileName
                        };

                        images.Add(brochure);
                    }
                }

                var resultcode = await _user.SaveBrocherImagesToDatabaseAsync(images, Convert.ToInt32(customerId));
                if (resultcode == 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion brocheure
    }
}

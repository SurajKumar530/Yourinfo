using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SixLabors.ImageSharp.Formats;
using Yourinfo.Models;
using Yourinfo.Repository.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Yourinfo.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _user;
        private readonly EmailService _emailService;
        private readonly string _logoPath = "wwwroot/images/logoQRCode.png";
        public UserController(IUserRepository user, EmailService emailService)
        {
            _user = user;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string dynamicText)
        {
            try
            {

                var modelCustomer = _user.cust_id(dynamicText);
                if (modelCustomer == null)
                {
                    return RedirectToAction("AccessDenied","Account");
                }

                string customerId = Convert.ToString(modelCustomer.cus_id);
                
                // Set the layout based on the flag
                ViewData["Layout"] = !string.IsNullOrEmpty(customerId) ? "_UserLayout" : "_Layout";

                if (!string.IsNullOrEmpty(Convert.ToString(modelCustomer.cus_id)))
                {
                    HttpContext.Session.SetString("CustomerId", customerId);
                }

                var storedCustomerId = HttpContext.Session.GetString("CustomerId");
                var modelData = await _user.GetCustomerWithGalleryImages(storedCustomerId);

                if (modelData == null)
                {
                    return NotFound("Customer not found.");
                }

                return View(modelData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFooterContent()
        {
            try
            {
                var customerId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(customerId))
                {
                    return BadRequest("Customer ID is required.");
                }

                var modelData = await _user.GetCustomerWithGalleryImages(customerId);

                if (modelData == null)
                {
                    return NotFound("Customer not found.");
                }

                // Prepare footer content
                var footerContent = new
                {
                    CompanyName = "Yours-Info",
                    Address = $"{modelData.Address}, {modelData.City}, {modelData.States}, {modelData.CountryName} {modelData.Pincode}",
                    Phone = modelData.RegisteredPhoneNumber,
                    Email = modelData.RegisteredEmailId,
                    fullUrl = getQrCode(modelData.FullURL),
                    SocialLinks = new
                    {
                        Twitter = modelData.TwitterLink,
                        googleMap = modelData.GoogleMapLink,
                        Facebook = modelData.FacebookCode,
                        Instagram = modelData.InstagramLink,
                        youTube = modelData.Youtube,
                        LinkedIn = modelData.LinkdinLink,
                        whatsapp=modelData.WhatsappNumber,
                        phoneNumber=modelData.RegisteredPhoneNumber,
                        sms=modelData.RegisteredPhoneNumber,
                        email=modelData.RegisteredEmailId
                    },
                    UsefulLinks = new[]
                    {
                        new { Text = "Home", Url = "#header" },
                        new { Text = "About us", Url = "#about" },
                        new { Text = "Services", Url = "#services" },
                        new { Text = "Portfolio", Url = "#portfolio" },
                        new { Text = "Team", Url = "#team" }
                    }
                };

                return Json(footerContent);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string name, string recipientEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            await _emailService.SendEmailAsync(recipientEmail, subject, message);
            ViewBag.Message = "Email sent successfully!";
            return View();
        }

        private string getQrCode(string qrcode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Initialize QR code generator and create the QR code data
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                // Generate the QR code bitmap
                using (Bitmap qrCodeBitmap = qrCode.GetGraphic(20))
                {
                    // Load the logo image
                    Bitmap logoBitmap;
                    using (FileStream logoStream = new FileStream(_logoPath, FileMode.Open, FileAccess.Read))
                    {
                        logoBitmap = new Bitmap(logoStream);
                    }

                    // Define logo size and position
                    int logoSize = qrCodeBitmap.Width / 5; // Logo size as a fraction of QR code size
                    int logoPositionX = (qrCodeBitmap.Width - logoSize) / 2;
                    int logoPositionY = (qrCodeBitmap.Height - logoSize) / 2;

                    // Draw the logo on top of the QR code
                    using (Graphics g = Graphics.FromImage(qrCodeBitmap))
                    {
                        // Optionally, add some transparency to the logo
                        ColorMatrix colorMatrix = new ColorMatrix
                        {
                            Matrix33 = 100f // Adjust logo transparency (0.0f to 1.0f)
                        };
                        ImageAttributes imgAttr = new ImageAttributes();
                        imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        // Resize the logo image
                        using (Bitmap resizedLogo = new Bitmap(logoBitmap, new Size(logoSize, logoSize)))
                        {
                            // Draw the resized logo image on top of the QR code
                            g.DrawImage(resizedLogo, new Rectangle(logoPositionX, logoPositionY, logoSize, logoSize),
                                0, 0, resizedLogo.Width, resizedLogo.Height, GraphicsUnit.Pixel, imgAttr);
                        }
                    }

                    // Save the final image to memory stream
                    qrCodeBitmap.Save(ms, ImageFormat.Png);
                    return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            
        }

    }

}

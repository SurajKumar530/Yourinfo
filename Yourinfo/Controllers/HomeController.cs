using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Utility;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yourinfo.Models;
using Yourinfo.Repository.Interface;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Yourinfo.Repository.Repo;
using SixLabors.ImageSharp.Formats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using SixLabors.ImageSharp.Formats.Jpeg;
using MailKit;


namespace Yourinfo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _user;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ImageService imageService;
        public HomeController(ApplicationDbContext dbContext, IUserRepository user, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, ImageService _imageService)
        {
            _context = dbContext;
            _user = user;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            imageService = _imageService;
        }
        public IActionResult Index()
        {

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (model.Username == null && model.Password == null)
                {
                    return BadRequest("Invalid Request ");
                }

                User user = await _user.AuthenticateUser(model.Username, model.Password);

                if (user != null)
                {
                    return RedirectToAction("createWebSite", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult createWebSite()
        {
            try
            {
                var model = new CustomerMSt();
                model.userViewChoose.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_GetChooseList");
                model.userViewCountry.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_GetCountryList");
                model.userViewAbout.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_AboutUSList");
                model.userViewCategory.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_CategoryList");

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request !!", ex);
                throw ex;
            }
        }

        public JsonResult GetStates(string countrytext)
        {
            try
            {
                var model = new CustomerMSt();
                model.userViewState.DropdownItems = _user.GetSelectedListAsync("GetStatesByCountry", countrytext);
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request !!", ex);
                throw;
            }

        }

        public JsonResult GetCity(string Statetext)
        {
            try
            {
                var model = new CustomerMSt();
                model.userViewCity.DropdownItems = _user.GetSelectedListAsync("GetCityByState", Statetext);
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request !!", ex);
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateWithImages([FromForm] CustomerMSt customerMSt, IFormFile Logofile, List<IFormFile> galleryFiles, List<IFormFile> BrochureFiles)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(new { success = false, message = "Model validation failed" });
                //}

                CustomerDetails customerViewModel = customerMSt.customer;
                var Domain = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                // Handle Logo file upload
                if (Logofile != null)
                {

                    try
                    {
                        customerViewModel.logoPath = await imageService.SaveImageAsync(Logofile, "uploads/Logo", 140, 140);
                        customerViewModel.Logo = Logofile.FileName;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error occurred while uploading the logo file:", ex);
                        return StatusCode(500, new { success = false, message = "Error occurred while uploading the logo file" });
                    }
                }

                // Handle Brochure file upload
                if (BrochureFiles != null && BrochureFiles.Count > 0)
                {
                    foreach (var file in BrochureFiles)
                    {
                        try
                        {
                            var brochure = new BrochureImages
                            {
                                ImagePath = await imageService.SaveImageAsync(file, "uploads/Brochure", 255, 255),
                                ImageName = file.FileName
                            };
                            customerViewModel.BrochureImage.Add(brochure);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error occurred while uploading the brochure file {file.FileName}:", ex);
                            return StatusCode(500, new { success = false, message = $"Error occurred while uploading the brochure file {file.FileName}" });
                        }
                    }
                }

                // Handle Gallery images file uploads
                if (galleryFiles != null && galleryFiles.Count > 0)
                {
                    foreach (var file in galleryFiles)
                    {
                        try
                        {
                            var gallery = new GalleryImages
                            {
                                ImagePath = await imageService.SaveImageAsync(file, "uploads/galleryLogo", 350, 262),
                                ImageName = file.FileName
                            };
                            customerViewModel.GalleryImages.Add(gallery);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error occurred while uploading the gallery file {file.FileName}:", ex);
                            return StatusCode(500, new { success = false, message = $"Error occurred while uploading the gallery file {file.FileName}" });
                        }
                    }
                }


                var result = _user.SaveCustomerWithImages(customerViewModel, Domain);

                if (result.Result.StatusCode == 0)
                {

                    try
                    {
                        if (galleryFiles != null && galleryFiles.Count > 0)
                        {
                            await _user.SaveImagesToDatabaseAsync(customerMSt.customer.GalleryImages, result.Result.customerID);
                        }

                        if (BrochureFiles != null && BrochureFiles.Count > 0)
                        {
                            await _user.SaveBrocherImagesToDatabaseAsync(customerViewModel.BrochureImage, result.Result.customerID);
                        }

                        string otpCode = GenerateOTP();
                        var otp = new OTP
                        {
                            CustomerID = result.Result.customerID,
                            MobileNumber = customerViewModel.RegisteredPhoneNumber,
                            OTPCode = otpCode,
                            OTPType = "Mobile",
                            CreatedAt = DateTime.Now,
                            Email = customerViewModel.RegisteredEmailId,
                            ExpiredAt = DateTime.Now.AddMinutes(10)
                        };
                        _user.SaveOTP(otp);

                        VerifyOTPViewModel verifyOTPView = new VerifyOTPViewModel
                        {
                            MobileIdentifier = customerViewModel.RegisteredPhoneNumber,
                            OTPMobileType = "Mobile",
                            EmailIdentifier = customerViewModel.RegisteredEmailId,
                            OTPEmailType = "Email"
                        };

                        _logger.LogInformation($"Customer created successfully with ID: {result.Result.customerID}");

                        return Json(new { success = true, redirectUrl = Url.Action("VerifyOTPPage", "Home", new { CustomerId = SiteUtility.Encrypt(Convert.ToString(result.Result.customerID)) }) });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error occurred while saving customer and related data:", ex);
                        return StatusCode(500, new { success = false, message = "Error occurred while saving customer and related data" });
                    }
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Failed to save customer data" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while processing form submission:", ex);
                ViewBag.Error = "Error occurred: " + ex.Message;
                return View(ex);
            }
        }


        //Action preview View
        public async Task<IActionResult> PreviewDetailsPage(string CustomerID)
        {
            if (string.IsNullOrEmpty(CustomerID))
            {
                _logger.LogWarning("CustomerID is null or empty.");
                return BadRequest("Invalid CustomerID.");
            }

            try
            {
                var CustomerId1 = SiteUtility.Decrypt(CustomerID);

                if (string.IsNullOrEmpty(CustomerId1))
                {
                    _logger.LogWarning("Decrypted CustomerID is null or empty.");
                    return BadRequest("Invalid CustomerID.");
                }

                var model = await _user.GetCustomerWithGalleryImages(CustomerId1);

                if (model == null)
                {
                    _logger.LogWarning($"No customer found for ID: {CustomerId1}");
                    return NotFound("Customer not found.");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching details for CustomerID: {CustomerID}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Action to display VerifyOTPPage
        [HttpGet]
        public IActionResult VerifyOTPPage(string CustomerId)
        {
            try
            {
                if (string.IsNullOrEmpty(CustomerId))
                {
                    _logger.LogError("Customer Id Not Found");
                    ViewBag.Error = "Customer Id is missing or invalid.";
                    return View();
                }

                var model = _user.verifyOTPView(SiteUtility.Decrypt(CustomerId));
                if (model == null)
                {
                    _logger.LogError($"No data found for Customer Id: {CustomerId}");
                    ViewBag.Error = "No data found for the provided Customer Id.";
                    return View();
                }

                return View("VerifyOTPPage", model);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while displaying Verify OTP Page:", ex);
                ViewBag.Error = "An error occurred while processing your request. Please try again later.";
                return View();
            }
        }

        [HttpPost]
        public IActionResult ResendOTP([FromBody] string customerId)
        {
            try
            {
                if (customerId == null || string.IsNullOrEmpty(customerId))
                {
                    return Json(new { success = false, message = "Invalid request parameters" });
                }

                var OTPValue = GenerateOTP();
                var resendOTP = _user.resendOTP(customerId, OTPValue);
                if (resendOTP == "1")
                {
                    return Json(new { success = true, message = "OTP has been resent successfully." });
                }
                else
                {
                    _logger.LogError("Failed to resend OTP.");
                    return Json(new { success = false, message = "Failed to resend OTP. Please try again later." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while resending OTP:", ex);
                return Json(new { success = false, message = "An error occurred while resending OTP." });
            }
        }

        // Action to verify OTP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOTP(string identifier, string otp, string otpType, string CustomerId)
        {
            try
            {

                var isVerified = _user.VerifyOTP(identifier, otp, otpType);
                if (isVerified == 1)
                {
                    _logger.LogInformation("OTP verified successfully.");
                    return Json(new { success = true, message = "OTP verified successfully.", 
                            redirectUrl = Url.Action("CreateLink", new { Link = "test", CustomerId = SiteUtility.Encrypt(CustomerId), EmailId = SiteUtility.Encrypt(identifier) }) });
                }
                else
                {
                    _logger.LogError("Invalid OTP.");
                    return Json(new { success = false, message = "Invalid OTP. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while Verify OTP:", ex);
                return Json(new { success = false, message = "An error occurred while Verify OTP." });
            }            
        }

        private string GenerateOTP()
        {
            // Generate a random OTP (you can customize this according to your needs)
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public IActionResult CreateLink(string Link, string CustomerId, string EmailId)
        {
            try
            {
                var CustomerId1 = SiteUtility.Decrypt(CustomerId);
                var identifier = SiteUtility.Decrypt(EmailId);
                LinkModel linkModel = new LinkModel();
                linkModel.Domain = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                linkModel.subdomain = Link;
                linkModel.CustomerId = CustomerId;
                linkModel.RegisterEmailId = EmailId;
                return View(linkModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request !!", ex);
                throw;
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLinkwithURL(string subDomain, string domain, string customerId)
        {
            try
            {
                var cust = SiteUtility.Decrypt(customerId);
                var result = _user.updateSubDomain(subDomain, domain, cust);
                if (result == 0)
                {
                    var URL = domain + '/' + subDomain;
                    _logger.LogInformation($"Create Link URL : {URL}");
                    var cust1 = SiteUtility.Encrypt(customerId);
                    return Json(new { success = true, url = URL, customerId = cust1 });
                }
                else if (result == 1)
                {
                    _logger.LogError($"Customer already exists with subdomain name");
                    return Json(new { success = false, message = "Customer already exists with the subdomain name." });
                }
                else
                {
                    _logger.LogError($"Customer not found");
                    return Json(new { success = false, message = "Customer not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Invalid request", ex);
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }


        [HttpGet]
        public IActionResult SuccessLink(string url, string customerId)
        {
            try
            {
                LinkModel model = new LinkModel();
                model.CustomerId = customerId;
                model.URL = url;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request !!", ex);
                throw;
            }
        }

    }
}


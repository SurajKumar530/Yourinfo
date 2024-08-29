using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yourinfo.Models;
using Yourinfo.Models.Admin;
using Yourinfo.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp.Processing;
using MailKit;

namespace Yourinfo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminRepository _admin;
        private readonly IUserRepository _user;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AdminController> _logger;
        private readonly ImageService imageService;

        public AdminController(ApplicationDbContext dbContext, IAdminRepository admin, IWebHostEnvironment hostingEnvironment, ILogger<AdminController> logger, IUserRepository user, ImageService _imageService)
        {
            _context = dbContext;
            _admin = admin;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _user = user;
            imageService = _imageService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index(string keyword, int page = 1, int pageSize = 4)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Admin");
                }
                var modelData = _admin.getAllCustomerDetails(keyword, page, pageSize);
                return View(modelData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving customer details: {ex.Message}");
                throw;
            }
        }

        [AllowAnonymous] // Allows access without authentication
        [HttpPost]
        public IActionResult Search(string keyword, int page = 1, int pageSize = 10)
        {
            try
            {
                var modelData = _admin.getAllSearchCustomerDetails(keyword, page, pageSize);
                return View("Index", modelData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching customer details: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Admin")] // Only users in "Admin" role can access
        [HttpPost]
        public IActionResult VerifyAction(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // If the user is not authenticated
            }
            try
            {
                var result = _admin.VerifyCustomer(id);
                var message = result == 1 ? $"Activate successful Customer for ID {id}" :
                              result == 2 ? $"De-Activate Customer for ID {id}" :
                              $"Error While Procced Customer for ID {id}";

                return Json(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying customer: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Admin")] // Only users in "Admin" role can access
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            try
            {
                var result = _admin.UpdateStatusCustomer(id);
                var message = result == 1 || result == 2 ?
                              $"Status Change Customer for ID {id}" :
                              $"Error While Procced Customer for ID {id}";

                return Json(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating customer status: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Admin")] // Only users in "Admin" role can access
        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            try
            {
                var result = _admin.DeleteCustomer(id);
                var message = result == 1 || result == 2 ?
                              $"Delete Customer for ID {id}" :
                              $"Error While Procced Customer for ID {id}";

                return Json(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting customer: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Admin")] // Only users in "Admin" role can access
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _admin.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            var model = GetDropDownData();
            var viewModel = new CustomerMSt
            {
                customer = customer.Result,
                userViewChoose = model.userViewChoose,
                userViewCountry = model.userViewCountry,
                userViewAbout = model.userViewAbout,
                userViewCategory=model.userViewCategory
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")] // Only users in "Admin" role can access
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomerDet(int cus_id, CustomerDetails customer, IFormFile Logofile, List<IFormFile> galleryFiles, List<IFormFile> BrochureFiles)
        {
            try
            {
                if (cus_id == 0)
                {
                    if (customer.cus_id != 0)
                    {
                        cus_id = customer.cus_id;
                    }
                    else
                    {
                        return BadRequest("Customer Not Found");
                    }
                }

                // Handle Logo file upload
                if (Logofile != null)
                {

                    customer.logoPath =await imageService.SaveImageAsync(Logofile, "uploads/Logo", 140, 140);
                    customer.Logo = Logofile.FileName;

                    //// Generate a unique file name
                    //var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Logofile.FileName);
                    //var _uploadLogoFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "Logo");
                    //var filePath = Path.Combine(_uploadLogoFolder, uniqueFileName);

                    ////using var streamFile = Logofile.OpenReadStream();
                    ////var compressedImage = await CompressImageAsync(streamFile);

                    //// Resize and save the image
                    //using (var image = Image.Load(Logofile.OpenReadStream()))
                    //{
                    //    // Resize the image to 736 × 426 pixels
                    //    image.Mutate(x => x.Resize(140, 140));
                    //    var jpegEncoder = new JpegEncoder
                    //    {
                    //        Quality = 90
                    //    };

                    //    // Save the resized and compressed image
                    //    await image.SaveAsync(filePath, jpegEncoder);
                    //}


                    //// Save the file
                    //using (var stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await Logofile.CopyToAsync(stream);
                    //    customerViewModel.Logo = Logofile.FileName;
                    //    customerViewModel.logoPath = "/uploads/Logo/" + uniqueFileName;
                    //}
                }

                // Handle Brochure file upload
                if (BrochureFiles != null && BrochureFiles.Count > 0)
                {
                    foreach (var file in BrochureFiles)
                    {
                        var brochure = new BrochureImages
                        {
                            ImagePath = await imageService.SaveImageAsync(file, "uploads/Brochure", 255, 255),
                            ImageName = file.FileName
                        };
                        customer.BrochureImage.Add(brochure);

                        //var brochure = new BrochureImages();
                        //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        //var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "Brochure");
                        //var filePath = Path.Combine(uploadsFolder, fileName);

                        //if (!Directory.Exists(uploadsFolder))
                        //{
                        //    Directory.CreateDirectory(uploadsFolder);
                        //}

                        //// Resize and save the image
                        //using (var image = Image.Load(file.OpenReadStream()))
                        //{
                        //    image.Mutate(x => x.Resize(140, 140));
                        //    var jpegEncoder = new JpegEncoder
                        //    {
                        //        Quality = 100
                        //    };

                        //    // Save the resized and compressed image
                        //    await image.SaveAsync(filePath, jpegEncoder);
                        //}

                        //using (var stream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await file.CopyToAsync(stream);
                        //    brochure.ImageName = file.FileName;
                        //    brochure.ImagePath = "/uploads/Brochure/" + fileName;
                        //}

                        //customerViewModel.BrochureImage.Add(brochure);
                    }
                }

                // Handle Gallery images file uploads
                if (galleryFiles != null && galleryFiles.Count > 0)
                {
                    foreach (var file in galleryFiles)
                    {
                        var gallery = new GalleryImages
                        {
                            ImagePath = await imageService.SaveImageAsync(file, "uploads/galleryLogo", 350, 262),
                            ImageName = file.FileName
                        };
                        customer.GalleryImages.Add(gallery);

                        //var gallery = new GalleryImages();

                        //// Generate a unique file name
                        //var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        //var _uploadGalleryFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "galleryLogo");
                        //var filePath = Path.Combine(_uploadGalleryFolder, uniqueFileName);

                        //// Resize and save the image
                        //using (var image = Image.Load(file.OpenReadStream()))
                        //{
                        //    image.Mutate(x => x.Resize(350, 262));
                        //    var jpegEncoder = new JpegEncoder
                        //    {
                        //        Quality = 100
                        //    };

                        //    // Save the resized and compressed image
                        //    await image.SaveAsync(filePath, jpegEncoder);
                        //}

                        //// Save the file
                        //using (var stream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await file.CopyToAsync(stream);
                        //    gallery.ImageName = file.FileName;
                        //    gallery.ImagePath = "/uploads/galleryLogo/" + uniqueFileName;
                        //}

                        //customerViewModel.GalleryImages.Add(gallery);
                    }
                }

                var result = _admin.uPdateCustomerByID(cus_id, customer);

                if (result.Result == 1)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Json(new { message = $"Error While Procced Customer for ID {cus_id}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error editing customer details: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Admin")] // Only users in "Admin" role can access
        [HttpPost]
        public async Task<IActionResult> UpdateDays(int customerId, int additionalDays)
        {
            var result = await _admin.ExtendExpiryDateAsync(customerId, additionalDays);

            if (result == "Customer not found")
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [AllowAnonymous] // Allows access without authentication
        [HttpPost]
        public JsonResult GetStates(string countrytext)
        {
            var model = new CustomerMSt();
            model.userViewState.DropdownItems = _user.GetSelectedListAsync("GetStatesByCountry", countrytext);
            return Json(model);
        }

        [AllowAnonymous] // Allows access without authentication
        [HttpPost]
        public JsonResult GetCity(string statetext)
        {
            var model = new CustomerMSt();
            model.userViewCity.DropdownItems = _user.GetSelectedListAsync("GetCityByState", statetext);
            return Json(model);
        }
       
        private CustomerMSt GetDropDownData()
        {
            var model = new CustomerMSt();
            model.userViewChoose.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_GetChooseList");
            model.userViewCountry.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_GetCountryList");
            model.userViewAbout.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_AboutUSList");
            model.userViewCategory.DropdownItems = _user.GetDropdownItemsFromStoredProcedure("sp_CategoryList");

            return model;
        }
    }
}

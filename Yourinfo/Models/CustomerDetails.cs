using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yourinfo.Models
{


    public class CustomerDetails
    {

        //[Required(ErrorMessage = "Customer Name Is Valid ")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please Select Choose Type")]
        [Display(Name = "Choose Type")]
        public string Chooseyourtype { get; set; }
        [Display(Name = "Header Type")]
        public string ChooseHeadertype { get; set; }

        [Required(ErrorMessage = "Company Bussiness Name is required")]
        public string Company_Business_Name { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }

        public string? GSTIN_Number { get; set; }

        [Required(ErrorMessage = "Owner Founder Name is required")]
        public string OwnerFounder_Name { get; set; }

        [Required(ErrorMessage = "Custom Field Name is required")]
        public string CustomField { get; set; }
        public string Logo { get; set; }
        public string logoPath { get; set; }
        public string GalleryImage { get; set; }
        public List<GalleryImages> GalleryImages { get; set; } = new List<GalleryImages>();

        [Required(ErrorMessage = "About Service is required")]
        public string AboutService { get; set; }

        [Required(ErrorMessage = "About US is required")]
        public string AboutUs { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
  

        public string? BankDetails { get; set; }

        public string OnlinePaymentLink { get; set; }

        [Required(ErrorMessage = "Please Select Country")]
        public string CountryName { get; set; }
        [Required(ErrorMessage = "Please Select State")]
        public string States { get; set; }

        [Required(ErrorMessage = "Please Select City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pincode is required")]
        [Range(100000, 999999, ErrorMessage = "Invalid Pincode")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Registered Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Registered Email Address")]
        public string RegisteredEmailId { get; set; }


        public string? CountryCode { get; set; }

        [Required(ErrorMessage = "Registered Phone number is required")]
        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Registered Phone number must be between 1 and 10 digits")]
        public string RegisteredPhoneNumber { get; set; }

        [Required(ErrorMessage = "Whatsapp number is required")]
        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Whatsapp number must be between 1 and 10 digits")]
        public string WhatsappNumber { get; set; }

        [Required(ErrorMessage = "Please Select Category")]
        public string CategoryList { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? GoogleMapLink { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? WebsiteName { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? FacebookCode { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? TwitterLink { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? Youtube { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? InstagramLink { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string? LinkdinLink { get; set; }
        public string? Brochure { get; set; }

        public List<BrochureImages> BrochureImage { get; set; } = new List<BrochureImages>();

        public string? SponsorCode { get; set; }

        [Display(Name = "Terms and Conditions")]
        [Required(ErrorMessage = "Please accept the terms and condition.")]
        public bool Term_Condition { get; set; }

        public Int32 cus_id { get; set; }
        public string FullURL { get; set; }

    }

    public class GalleryImages
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
    }
    
    public class BrochureImages
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
    }


    // Model to represent dropdown items
    public class DropdownItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    // View model for your form
    public class UserViewModel
    {
        public string SelectedItem { get; set; }
        public List<DropdownItem> DropdownItems { get; set; }
    }

    public class CustomerMSt
    {
        public CustomerMSt()
        {
            customer = new CustomerDetails();
            userViewChoose = new UserViewModel();
            userViewCountry = new UserViewModel();
            userViewState = new UserViewModel();
            userViewCity = new UserViewModel();
            userViewAbout = new UserViewModel();
            userViewCategory = new UserViewModel();
        }
        public CustomerDetails customer { get; set; }
        public UserViewModel userViewChoose { get; set; }
        public UserViewModel userViewCountry { get; set; }
        public UserViewModel userViewState { get; set; }
        public UserViewModel userViewCity { get; set; }
        public UserViewModel userViewAbout { get; set; }
        public UserViewModel userViewCategory { get; set; }
    }

    public class FileUploadViewModel
    {
        public List<IFormFile> Files { get; set; }
    }
}
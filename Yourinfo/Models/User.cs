using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Yourinfo.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255)]
        public string FullName { get; set; }

        public DateTime RegistrationDate { get; set; }
        public List<string> Roles { get; set; }
    }

    public class OTP
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string OTPCode { get; set; }
        public string OTPType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsVerified { get; set; }
    }

    public class VerifyOTPViewModel
    {
        public string MobileIdentifier { get; set; }
        public string EmailIdentifier { get; set; }
        public string OTPCode { get; set; }
        public string OTPEmailType { get; set; }
        public string OTPMobileType { get; set; }
        public string CustomerId { get; set; }
    }

    public class LinkModel
    {
        public string Domain {get; set; }
        public string subdomain { get; set; }
        public string URL { get; set; }
        public string CustomerId { get; set; }
        public string RegisterEmailId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yourinfo.Models.Admin
{
    public class CustomerViewModel
    {
        public CustomerViewModel()
        {
            CustomerList = new List<CustomerDet>();
        }

        public List<CustomerDet> CustomerList { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
    public class CustomerDet
    {
        public string CustomerID { get; set; }
        public string REGISTEREDDATE { get; set; }
        public string ACCOUNT_TYPE { get; set; }
        public string Business_Name { get; set; }
        public string FULL_NAME { get; set; }
        public string REGISTER_EMAIL { get; set; }
        public string Phone_Number { get; set; }
        public string Password { get; set; }
        public string EXPIREDDATE { get; set; }
        public string DAYSLEFT { get; set; }
        public string FullURL { get; set; }
        public string Status { get; set; }
        public string IsLiveVerfied { get; set; }
    }
}

using Yourinfo.Models;

namespace Yourinfo.Repository.Interface
{ 
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        // Method to call stored procedure for user authentication
        Task LogUserLoginHistoryAsync(int userId, DateTime loginTime, DateTime? logoutTime, string deviceIP, string deviceName, string browserInfo, string loginStatus, string sessionToken);
        Task<User> AuthenticateUser(string username, string password);

        //sAVE cUSOTMER DATA 
        Task<(int StatusCode, int customerID)> SaveCustomerWithImages(CustomerDetails customer,string Domain);
        Task<int> SaveImagesToDatabaseAsync(List<GalleryImages> images,int customerID);
        Task<int> SaveBrocherImagesToDatabaseAsync(List<BrochureImages> images,int customerID);

        List<DropdownItem> GetDropdownItemsFromStoredProcedure(string procedureName);
       List<DropdownItem> GetSelectedListAsync(string procedureName,string selectedValue);

        void SaveOTP(OTP oTP);

        int VerifyOTP(string identifier, string otp, string otpType);

        int updateSubDomain(string subDomain,string domain, string customerId);

        Task<CustomerDetails> GetCustomerWithGalleryImages(string CustomerID);

        VerifyOTPViewModel verifyOTPView(string customerID);

        string resendOTP(string CustomerID, string OTPValue);
        CustomerDetails cust_id(string dynamicText);
    }
}

using Yourinfo.Models;
using Yourinfo.Models.Admin;

namespace Yourinfo.Repository.Interface
{
    public interface IAdminRepository
    {
        CustomerViewModel getAllCustomerDetails(string keyword, int page, int pageSize);
        CustomerViewModel getAllSearchCustomerDetails(string keyword, int page, int pageSize);

        int VerifyCustomer(int ID);
        int UpdateStatusCustomer(int ID);
        int DeleteCustomer(int ID);
        Task<CustomerDetails> GetCustomerById(int CustomerID);
        Task<int> uPdateCustomerByID(int customerId, CustomerDetails customer);

        Task<string> ExtendExpiryDateAsync(int customerId, int additionalDays);

        Task<User> AuthenticateUser(string username, string password);

        Task<int> getUser(string email);
        Task<int> ResetPassword(string email,string password);
    }
}

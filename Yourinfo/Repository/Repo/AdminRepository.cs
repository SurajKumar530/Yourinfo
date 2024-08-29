using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Yourinfo.Models;
using Yourinfo.Models.Admin;
using Yourinfo.Repository.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Yourinfo.Repository.Repo
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
        }

        public CustomerViewModel getAllCustomerDetails(string keyword, int page, int pageSize)
        {
            try
            {
                CustomerViewModel customerView = new CustomerViewModel();
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    const string readSp = "GetAllCustomersWithPagination";
                    var dyParam = new DynamicParameters();
                    dyParam.Add("@Keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword, DbType.String, ParameterDirection.Input);
                    dyParam.Add("@Page", page, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("@PageSize", pageSize, DbType.Int32, ParameterDirection.Input);
                    var cusotmerList = db.Query<CustomerDet>(readSp, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                    // Calculate total count of records for pagination
                    int totalCount = GetTotalCustomerCount(_connectionString);


                    customerView.CustomerList = cusotmerList;

                    // Pass data to view model or ViewBag for pagination
                    customerView.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    customerView.CurrentPage = page;

                    return customerView;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CustomerViewModel getAllSearchCustomerDetails(string keyword, int page, int pageSize)
        {
            try
            {
                CustomerViewModel customerView = new CustomerViewModel();
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    const string readSp = "GetAllCustomersWithPagination";
                    var dyParam = new DynamicParameters();
                    dyParam.Add("@Keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword, DbType.String, ParameterDirection.Input);
                    dyParam.Add("@Page", page, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("@PageSize", pageSize, DbType.Int32, ParameterDirection.Input);
                    var cusotmerList = db.Query<CustomerDet>(readSp, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                    // Calculate total count of records for pagination
                    int totalCount = GetTotalCustomerCount(_connectionString);


                    customerView.CustomerList = cusotmerList;

                    // Pass data to view model or ViewBag for pagination
                    customerView.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    customerView.CurrentPage = page;

                    return customerView;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Helper method to get total count of customers (for pagination)
        private int GetTotalCustomerCount(string connection)
        {
            int count = 0;
            using (var connectionString = new SqlConnection(connection))
            {
                connectionString.Open();
                using (SqlCommand command = new SqlCommand("GetCustomersCount", connectionString))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        count = (int)result;
                    }
                }
                return count;
            }
        }

        public int VerifyCustomer(int ID)
        {
            try
            {
                int errorCode = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("VerifyCustomer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CustomerId", ID);

                        // Output parameter for inserted ID
                        SqlParameter paramIsVerified = new SqlParameter("@IsVerified", SqlDbType.Int);
                        paramIsVerified.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramIsVerified);

                        command.ExecuteNonQuery();

                        errorCode = Convert.ToInt32(paramIsVerified.Value);
                    }
                }

                return errorCode;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int UpdateStatusCustomer(int ID)
        {
            try
            {
                int errorCode = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("UpdateStatusCustomer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CustomerId", ID);

                        // Output parameter for inserted ID
                        SqlParameter paramIsVerified = new SqlParameter("@IsVerified", SqlDbType.Int);
                        paramIsVerified.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramIsVerified);

                        command.ExecuteNonQuery();

                        errorCode = Convert.ToInt32(paramIsVerified.Value);
                    }
                }

                return errorCode;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int DeleteCustomer(int ID)
        {
            try
            {
                int errorCode = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("DeleteCustomer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CustomerId", ID);

                        // Output parameter for inserted ID
                        SqlParameter paramIsVerified = new SqlParameter("@IsVerified", SqlDbType.Int);
                        paramIsVerified.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramIsVerified);

                        command.ExecuteNonQuery();

                        errorCode = Convert.ToInt32(paramIsVerified.Value);
                    }
                }

                return errorCode;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<CustomerDetails> GetCustomerById(int CustomerID)
        {
            CustomerDetails customerDetails = null;
            List<GalleryImages> galleryImages = new List<GalleryImages>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                DataSet ds = new DataSet();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetAllCustomers";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerID", Convert.ToString(CustomerID));

                    SqlDataAdapter sqlData = new SqlDataAdapter(command);

                    await Task.Run(() => sqlData.Fill(ds));

                    if (ds.Tables[0].Rows.Count > 0)

                    {
                        foreach (DataRow reader in ds.Tables[0].Rows)
                        {
                            customerDetails = new CustomerDetails
                            {
                                CustomerName = reader["CustomerName"].ToString(),
                                Chooseyourtype = reader["Chooseyourtype"].ToString(),
                                ChooseHeadertype = reader["ChooseHeadertype"].ToString(),
                                Company_Business_Name = reader["Company_Business_Name"].ToString(),
                                Color = reader["Color"].ToString(),
                                TextColor = reader["TextColor"].ToString(),
                                GSTIN_Number = reader["GSTIN_Number"].ToString(),
                                OwnerFounder_Name = reader["OwnerFounder_Name"].ToString(),
                                CustomField = reader["CustomField"].ToString(),
                                Logo = reader["Logo"].ToString(),
                                logoPath = reader["logoPath"].ToString(),
                                GalleryImage = reader["GalleryImage"].ToString(),
                                AboutService = reader["AboutService"].ToString(),
                                AboutUs = reader["AboutUs"].ToString(),
                                Password = reader["Password"].ToString(),
                                ConfirmPassword = reader["ConfirmPassword"].ToString(),
                                Address = reader["Address"].ToString(),
                                BankDetails = reader["BankDetails"].ToString(),
                                OnlinePaymentLink = reader["OnlinePaymentLink"].ToString(),
                                CountryName = reader["CountryName"].ToString(),
                                States = reader["States"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToString(reader["Pincode"]),
                                RegisteredEmailId = reader["RegisteredEmailId"].ToString(),
                                CountryCode = reader["CountryCode"].ToString(),
                                RegisteredPhoneNumber = reader["RegisteredPhoneNumber"].ToString(),
                                WhatsappNumber = reader["WhatsappNumber"].ToString(),
                                CategoryList = reader["CategoryList"].ToString(),
                                GoogleMapLink = reader["GoogleMapLink"].ToString(),
                                WebsiteName = reader["WebsiteName"].ToString(),
                                FacebookCode = reader["FacebookCode"].ToString(),
                                TwitterLink = reader["TwitterLink"].ToString(),
                                Youtube = reader["Youtube"].ToString(),
                                InstagramLink = reader["InstagramLink"].ToString(),
                                LinkdinLink = reader["LinkdinLink"].ToString(),
                                Brochure = reader["Brochure"].ToString(),
                                SponsorCode = reader["SponsorCode"].ToString(),
                                FullURL = Convert.ToString(reader["FullURL"]),
                                cus_id = Convert.ToInt32(reader["cus_id"])
                            };
                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow reader in ds.Tables[1].Rows)
                        {
                            var galleryImage = new GalleryImages
                            {
                                ImageName = reader["ImageName"].ToString(),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            customerDetails.GalleryImages.Add(galleryImage);
                        }
                    };

                }
            }

            return customerDetails;
        }

        public async Task<int> uPdateCustomerByID(int customerId, CustomerDetails customer)
        {
            try
            {
                int statusCode = -1;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new SqlCommand("UpdateCustomerWithImages", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                // Customer parameters
                                command.Parameters.AddWithValue("@CustomerID", customerId);
                                command.Parameters.AddWithValue("@Name", customer.CustomerName);
                                command.Parameters.AddWithValue("@CustomerType", customer.Chooseyourtype);

                                command.Parameters.AddWithValue("@CustomerHeaderType", customer.ChooseHeadertype);
                                command.Parameters.AddWithValue("@Logo", customer.Logo);
                                command.Parameters.AddWithValue("@LogoPath", customer.logoPath);
                                command.Parameters.AddWithValue("@Company_Business_Name", customer.Company_Business_Name);
                                command.Parameters.AddWithValue("@GSTIN", customer.GSTIN_Number);

                                command.Parameters.AddWithValue("@Owner_Founder_Name", customer.OwnerFounder_Name);
                                command.Parameters.AddWithValue("@Custom_Field", customer.CustomField);
                                command.Parameters.AddWithValue("@About_Service_Name", customer.AboutService);
                                command.Parameters.AddWithValue("@About_Us", customer.AboutUs);
                                command.Parameters.AddWithValue("@Address", customer.Address);
                                command.Parameters.AddWithValue("@Bank_Details", customer.BankDetails);

                                command.Parameters.AddWithValue("@Payment_Link", customer.OnlinePaymentLink);
                                command.Parameters.AddWithValue("@Country_Name", customer.CountryName);
                                command.Parameters.AddWithValue("@State_Name", customer.States);
                                command.Parameters.AddWithValue("@City_Name", customer.City);
                                command.Parameters.AddWithValue("@Pincode", customer.Pincode);
                                command.Parameters.AddWithValue("@Reg_Email", customer.RegisteredEmailId);

                                command.Parameters.AddWithValue("@Password", customer.Password);
                                command.Parameters.AddWithValue("@Confirm_Password", customer.ConfirmPassword);
                                command.Parameters.AddWithValue("@Phone_Number", customer.RegisteredPhoneNumber);
                                command.Parameters.AddWithValue("@Whatapp_Number", customer.WhatsappNumber);
                                command.Parameters.AddWithValue("@Category", customer.CategoryList);

                                command.Parameters.AddWithValue("@Google_Link", customer.GoogleMapLink);
                                command.Parameters.AddWithValue("@Website_Name", customer.WebsiteName);
                                command.Parameters.AddWithValue("@Facebook_Link", customer.FacebookCode);
                                command.Parameters.AddWithValue("@Twitter_Link", customer.TwitterLink);
                                command.Parameters.AddWithValue("@Youtube_Link", customer.Youtube);

                                command.Parameters.AddWithValue("@Instagram_Link", customer.InstagramLink);
                                command.Parameters.AddWithValue("@Linkdin_Link", customer.LinkdinLink);
                                command.Parameters.AddWithValue("@Brochure", customer.Brochure);
                                command.Parameters.AddWithValue("@Sponsor_Code", customer.SponsorCode);
                                command.Parameters.AddWithValue("@Term_Condition", customer.Term_Condition == true ? "Y" : "N");

                                // Output parameter for stored procedure result
                                SqlParameter resultCode = new SqlParameter("@resultCode", SqlDbType.Int);
                                resultCode.Direction = ParameterDirection.Output;
                                command.Parameters.Add(resultCode);

                                await command.ExecuteNonQueryAsync();

                                statusCode = Convert.ToInt32(resultCode.Value);

                                if (statusCode == 1)
                                {
                                    // Update images
                                    if (customer.GalleryImages.Count > 0)
                                    {
                                        statusCode = await UpdateImagesToDatabaseAsync(customer.GalleryImages, customerId);

                                        if (statusCode == 1)
                                        {
                                            transaction.Commit();
                                            return statusCode; // Success
                                        }
                                        else
                                        {
                                            transaction.Rollback();
                                            return statusCode; // Failure in updating images
                                        }
                                    }
                                    else
                                    {
                                        transaction.Commit();
                                        return statusCode;
                                    }
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return statusCode; // Failure in updating customer details
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error executing database query", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing database query", ex);
            }
        }

        public async Task<int> UpdateImagesToDatabaseAsync(List<GalleryImages> images, int customerID)
        {
            int status = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    foreach (var model in images)
                    {
                        const string insertSP = "UPDATECustomerGalleryImages";

                        var dyparam = new DynamicParameters();
                        dyparam.Add("@CustomerID", customerID, DbType.Int32, ParameterDirection.Input);
                        dyparam.Add("@ImageName", model.ImageName, DbType.String, ParameterDirection.Input);
                        dyparam.Add("@ImagePath", model.ImagePath, DbType.String, ParameterDirection.Input);
                        dyparam.Add("@resultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        // Execute the insert operation
                        await connection.ExecuteAsync(insertSP, param: dyparam, commandType: CommandType.StoredProcedure);

                        status = dyparam.Get<int>("@resultCode");
                        if (status == 0)
                        {
                            return status; // Error in inserting images
                        }
                    }
                    return status; // Success
                }
                catch (Exception ex)
                {
                    throw new Exception("Error executing database query", ex);
                }
            }
        }


        public async Task<string> ExtendExpiryDateAsync(int customerId, int additionalDays)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ExtendExpiryDate", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    command.Parameters.AddWithValue("@AdditionalDays", additionalDays);

                    // Execute the command
                    var result = await command.ExecuteScalarAsync();

                    return result.ToString();
                }
            }
        }


        #region Login Logic
        public async Task<User> AuthenticateUser(string username, string password)
        {
            User user = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "sp_LoginUser";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
                        command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar) { Value = password });

                        await connection.OpenAsync();

                        // Execute the command and read the result
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read() && !reader.IsDBNull(reader.GetOrdinal("Id")))
                            {
                                user = new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Roles = new List<string>()
                                };

                                // Populate roles if available
                                do
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal("RoleName")))
                                    {
                                        user.Roles.Add(reader["RoleName"].ToString());
                                    }
                                } while (await reader.ReadAsync());
                            }
                        }
                    }
                }

                return user;
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions (e.g., connection failures, timeouts)
                // Log or throw as needed
                Console.WriteLine($"SQL Exception: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception occurred in UserRepository.AuthenticateUserAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> getUser(string email)
        {
            try
            {
                int status = -1;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        const string insertSP = "sp_GetUser";

                        var dyparam = new DynamicParameters();
                        dyparam.Add("@email", email, DbType.String, ParameterDirection.Input);
                        dyparam.Add("@V_COUNT", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        // Execute the insert operation
                        await connection.ExecuteAsync(insertSP, param: dyparam, commandType: CommandType.StoredProcedure);

                        status = dyparam.Get<int>("@V_COUNT");

                        return status; // Error in inserting images
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error executing database query", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing database query", ex);
            }
        }

        public async Task<int> ResetPassword(string email,string password)
        {
            try
            {
                int status = -1;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        const string insertSP = "[sp_ResetUserPassword]";

                        var dyparam = new DynamicParameters();
                        dyparam.Add("@email", email, DbType.String, ParameterDirection.Input);
                        dyparam.Add("@password", password, DbType.String, ParameterDirection.Input);
                        dyparam.Add("@V_COUNT", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        // Execute the insert operation
                        await connection.ExecuteAsync(insertSP, param: dyparam, commandType: CommandType.StoredProcedure);

                        status = dyparam.Get<int>("@V_COUNT");

                        return status; // Error in inserting images
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error executing database query", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing database query", ex);
            }
        }
        #endregion

        //public async Task<int> updateImagesToDatabaseAsync(List<GalleryImages> images, int customerID, SqlTransaction transaction = null)
        //{
        //    try
        //    {
        //        int status = -1;

        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            if (transaction == null)
        //            {
        //                await connection.OpenAsync();
        //            }
        //            else
        //            {
        //                connection.ConnectionString = _connectionString;
        //                connection.Open();
        //            }

        //            foreach (var model in images)
        //            {
        //                const string insertSP = "UPDATECustomerGalleryImages";

        //                var dyparam = new DynamicParameters();
        //                dyparam.Add("@CustomerID", customerID, DbType.Int32, ParameterDirection.Input);
        //                dyparam.Add("@ImageName", model.ImageName, DbType.String, ParameterDirection.Input);
        //                dyparam.Add("@ImagePath", model.ImagePath, DbType.String, ParameterDirection.Input);
        //                dyparam.Add("@resultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                // Execute the insert operation
        //                await connection.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);

        //                status = dyparam.Get<int>("@resultCode");
        //                if (status == 0)
        //                {
        //                    return status; // Error in inserting images
        //                }
        //            }

        //            return status; // Success
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error executing database query", ex);
        //    }
        //}


        //public int updateImagesToDatabaseAsync(List<GalleryImages> images, int customerID)
        //{
        //    int status = -1;
        //    try
        //    {
        //        using (IDbConnection db = new SqlConnection(_connectionString))
        //        {
        //            db.Open();
        //            using (var transaction = db.BeginTransaction())
        //            {
        //                try
        //                {
        //                    foreach (var model in images)
        //                    {
        //                        const string insertSP = "InsertImages";
        //                        var dyparam = new DynamicParameters();
        //                        dyparam.Add("@CustomerID", customerID, DbType.Int32, ParameterDirection.Input);
        //                        dyparam.Add("@ImageName", model.ImageName, DbType.String, ParameterDirection.Input);
        //                        dyparam.Add("@ImagePath", model.ImagePath, DbType.String, ParameterDirection.Input);
        //                        dyparam.Add("@resultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                        // Execute the insert operation
        //                        db.Execute(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);

        //                        status = dyparam.Get<int>("@resultCode");
        //                        if (status == 0)
        //                        {
        //                            return status;
        //                        }
        //                    }
        //                    return status;
        //                }
        //                catch (Exception ex)
        //                {
        //                    transaction.Rollback();
        //                    throw new Exception("Error executing database query", ex);
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        // Handle specific SQL exceptions
        //        throw new Exception("SQL Error executing database query", sqlEx);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any other unexpected exceptions
        //        throw new Exception("Error executing database query", ex);
        //    }
        //}
    }
}
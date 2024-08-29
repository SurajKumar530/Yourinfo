using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Utility;
using Yourinfo.Models;
using Yourinfo.Repository.Interface;

namespace Yourinfo.Repository.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "sp_getAllUsers"; // Name of your stored procedure

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                User user = new User
                                {
                                    Id = Convert.ToInt32(reader["USER_ID"]),
                                    Username = reader["Username"].ToString(),
                                    PasswordHash = reader["password_hash"].ToString(),
                                    Email = reader["Email"].ToString()
                                    // Populate other properties as needed
                                };

                                users.Add(user);
                            }
                        }
                    }
                }
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
                // Handle other exceptions
                // Log or throw as needed
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }

            return users;
        }

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
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString()
                                    // Populate other properties as needed
                                };
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

        public async Task LogUserLoginHistoryAsync(int userId, DateTime loginTime, DateTime? logoutTime, string deviceIP, string deviceName, string browserInfo, string loginStatus, string sessionToken)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sqlQuery = "InsertUserLoginHistory";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@LoginTime", loginTime);
                        command.Parameters.AddWithValue("@LogoutTime", (object)logoutTime ?? DBNull.Value); // Handle nullable logoutTime
                        command.Parameters.AddWithValue("@DeviceIP", deviceIP);
                        command.Parameters.AddWithValue("@DeviceName", deviceName);
                        command.Parameters.AddWithValue("@BrowserInfo", browserInfo);
                        command.Parameters.AddWithValue("@LoginStatus", loginStatus);
                        command.Parameters.AddWithValue("@SessionToken", sessionToken);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle and log exceptions as needed
                Console.WriteLine($"Error logging user login history: {ex.Message}");
                throw;
            }
        }

        // Method to call the stored procedure to save customer and images
        public async Task<(int StatusCode, int customerID)> SaveCustomerWithImages(CustomerDetails customer, string Domain)
        {
            try
            {
                int statusCode = -1;
                int customerID = -1;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("AddCustomerWithImages", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Customer parameters
                        command.Parameters.AddWithValue("@Name", customer.CustomerName);
                        command.Parameters.AddWithValue("@CustomerType", customer.Chooseyourtype);

                        command.Parameters.AddWithValue("@CustomerHeaderType", customer.ChooseHeadertype);
                        command.Parameters.AddWithValue("@Logo", customer.Logo);
                        command.Parameters.AddWithValue("@LogoPath", customer.logoPath);
                        command.Parameters.AddWithValue("@Color", customer.Color);
                        command.Parameters.AddWithValue("@TextColor", customer.TextColor);
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
                        command.Parameters.AddWithValue("@Domain", Domain);

                        // Output parameter for inserted ID
                        SqlParameter outputParam = new SqlParameter("@customerIDs", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputParam);

                        // Output parameter for inserted ID
                        SqlParameter resultCode = new SqlParameter("@resultCode", SqlDbType.Int);
                        resultCode.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultCode);

                        command.ExecuteNonQuery();
                        statusCode = Convert.ToInt32(resultCode.Value);
                        customerID = Convert.ToInt32(outputParam.Value);// Assuming ErrorCode is the second column
                        return (statusCode, customerID);

                    }
                }
                return (statusCode, customerID);
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, rethrow, etc.)
                throw new Exception("Error executing database query", ex);
            }
        }

        // Method to retrieve dropdown items from stored procedure
        public List<DropdownItem> GetDropdownItemsFromStoredProcedure(string procedureName)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var dyparam = new DynamicParameters();
                    return db.Query<DropdownItem>(Convert.ToString(procedureName), param: dyparam, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
               // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public List<DropdownItem> GetSelectedListAsync(string procedureName, string selectedValue)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var dyparam = new DynamicParameters();
                    dyparam.Add("@selectedValue", selectedValue, DbType.String, ParameterDirection.Input);
                    return db.Query<DropdownItem>(Convert.ToString(procedureName), param: dyparam, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        //// Method to save images to database using stored procedure
        //public async Task<int> SaveImagesToDatabaseAsync(List<GalleryImages> images, int customerID)
        //{
        //    int status = -1;
        //    try
        //    {
        //        using (IDbConnection db = new SqlConnection(_connectionString))
        //        {
        //            foreach (var model in images)
        //            {
        //                const string readSP = "InsertImages";
        //                var dyparam = new DynamicParameters();
        //                dyparam.Add("@CustomerID", customerID, DbType.String, ParameterDirection.Input);
        //                dyparam.Add("@ImageName", model.ImageName, DbType.String, ParameterDirection.Input);
        //                dyparam.Add("@ImagePath", model.ImagePath, DbType.String, ParameterDirection.Input);
        //                var result = db.QueryMultiple(readSP, param: dyparam, commandType: CommandType.StoredProcedure);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error executing database query", ex);
        //    }
        //    return status;
        //}

        public async Task<int> SaveImagesToDatabaseAsync(List<GalleryImages> images, int customerID)
        {
            int status = -1;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            foreach (var model in images)
                            {
                                const string insertSP = "InsertImages";
                                var dyparam = new DynamicParameters();
                                dyparam.Add("@CustomerID", customerID, DbType.Int32, ParameterDirection.Input);
                                dyparam.Add("@ImageName", model.ImageName, DbType.String, ParameterDirection.Input);
                                dyparam.Add("@ImagePath", model.ImagePath, DbType.String, ParameterDirection.Input);

                                // Execute the insert operation
                                await db.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);
                            }

                            // Commit transaction if all inserts succeed
                            transaction.Commit();
                            status = 0; // Or any success code
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error executing database query", ex);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle specific SQL exceptions
                throw new Exception("SQL Error executing database query", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                throw new Exception("Error executing database query", ex);
            }
            return status;
        }


        public async Task<int> SaveBrocherImagesToDatabaseAsync(List<BrochureImages> images, int customerID)
        {
            int status = -1;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            foreach (var model in images)
                            {
                                const string insertSP = "InsertBrochureImages";
                                var dyparam = new DynamicParameters();
                                dyparam.Add("@CustomerID", customerID, DbType.Int32, ParameterDirection.Input);
                                dyparam.Add("@ImageName", model.ImageName, DbType.String, ParameterDirection.Input);
                                dyparam.Add("@ImagePath", model.ImagePath, DbType.String, ParameterDirection.Input);

                                // Execute the insert operation
                                await db.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);
                            }

                            // Commit transaction if all inserts succeed
                            transaction.Commit();
                            status = 0; // Or any success code
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error executing database query", ex);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle specific SQL exceptions
                throw new Exception("SQL Error executing database query", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                throw new Exception("Error executing database query", ex);
            }
            return status;
        }

        public void SaveOTP(OTP otpmodel)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SaveOTP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CustomerId", otpmodel.CustomerID);
                    command.Parameters.AddWithValue("@MobileNumber", otpmodel.MobileNumber);
                    command.Parameters.AddWithValue("@Email", otpmodel.Email);
                    command.Parameters.AddWithValue("@OTPCode", otpmodel.OTPCode);
                    command.Parameters.AddWithValue("@OTPType", otpmodel.OTPType);
                    command.Parameters.AddWithValue("@CreatedAt", otpmodel.CreatedAt);
                    command.Parameters.AddWithValue("@ExpiredAt", otpmodel.ExpiredAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public int VerifyOTP(string identifier, string otp, string otpType)
        {
            int isVerified = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("VerifyOTP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Identifier", identifier);
                    command.Parameters.AddWithValue("@OTP", otp);
                    command.Parameters.AddWithValue("@OTPType", otpType);

                    // Output parameter for inserted ID
                    SqlParameter paramIsVerified = new SqlParameter("@IsVerified", SqlDbType.Int);
                    paramIsVerified.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramIsVerified);

                    command.ExecuteNonQuery();

                    isVerified = Convert.ToInt32(paramIsVerified.Value);
                }
            }

            return isVerified;
        }

        public int updateSubDomain(string subDomain, string domain, string customerId)
        {
            try
            {
                int ErrorCode = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("UpdateURLSubDomain", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        command.Parameters.AddWithValue("@Domain", domain);
                        command.Parameters.AddWithValue("@SubDomain", subDomain);

                        // Output parameter for inserted ID
                        SqlParameter paramErrorCode = new SqlParameter("@ErrorCode", SqlDbType.Int);
                        paramErrorCode.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramErrorCode);

                        command.ExecuteNonQuery();

                        ErrorCode = Convert.ToInt32(paramErrorCode.Value);
                    }
                }
                return ErrorCode;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomerDetails> GetCustomerWithGalleryImages(string CustomerID)
        {
            CustomerDetails customerDetails = null;
            List<GalleryImages> galleryImages = new List<GalleryImages>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                DataSet ds = new DataSet();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetCustomerWithGalleryImages";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerID", CustomerID);

                    SqlDataAdapter sqlData = new SqlDataAdapter(command);

                   await Task.Run(() => sqlData.Fill(ds) );

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
                                FullURL = reader["FullURL"].ToString()
                            };
                        }
                    }

                    if(ds.Tables[1].Rows.Count> 0)
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

                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow reader in ds.Tables[2].Rows)
                        {
                            var brochureImage = new BrochureImages
                            {
                                ImageName = reader["ImageName"].ToString(),
                                ImagePath = reader["ImagePath"].ToString()
                            };
                            customerDetails.BrochureImage.Add(brochureImage);
                        }
                    };
                }
            }

            return customerDetails;
        }

        public VerifyOTPViewModel verifyOTPView(string customerID)
        {

            try
            {

                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string readSp = "getOTPDet";
                    var dyParam = new DynamicParameters();
                    dyParam.Add("@customerID", customerID, DbType.String, ParameterDirection.Input);
                    var result = db.QueryMultiple(readSp, param: dyParam, commandType: CommandType.StoredProcedure);
                    return result.Read<VerifyOTPViewModel>().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public string resendOTP(string CustomerID, string OTPValue)
        {
            try
            {
                string isVerified = "0";

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("ResendOTP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CustomerID", CustomerID);
                        command.Parameters.AddWithValue("@OTP", OTPValue);
                        // Output parameter for inserted ID
                        SqlParameter paramErrorCode = new SqlParameter("@ErrorCode", SqlDbType.Int);
                        paramErrorCode.Direction = ParameterDirection.Output;
                        command.Parameters.Add(paramErrorCode);

                        command.ExecuteNonQuery();

                        isVerified = Convert.ToString(paramErrorCode.Value);
                    }
                }

                return isVerified;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing database query", ex);
            }
        }

        public CustomerDetails cust_id(string dynamicText)
        {
            try
            {
               
                using (IDbConnection db = new SqlConnection(_connectionString))
                {

                    const string readSp = "[sp_getcust_idfromsubdomain]";
                    var dyParam = new DynamicParameters();
                    dyParam.Add("@dynamicText", dynamicText, DbType.String, ParameterDirection.Input);
             

                    var readResult = db.QueryMultiple(readSp, param: dyParam, commandType: CommandType.StoredProcedure);


                    return readResult.Read<CustomerDetails>().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
       

                return null;
            }
            //try
            //{

            //    using (IDbConnection db = new SqlConnection(_connectionString))
            //    {
            //        const string readSp = "sp_getcust_idfromsubdomain";
            //        var dyParam = new DynamicParameters();
            //        dyParam.Add("@dynamicText", dynamicText, DbType.String, ParameterDirection.Input);
            //        var result = db.QueryMultiple(readSp, param: dyParam, commandType: CommandType.StoredProcedure);
            //        return result.Read<CustomerDetails>().FirstOrDefault(); 
            //    }
            //}
            //catch (Exception ex)
            //{

            //    return null;
            //}
        }
    }
}

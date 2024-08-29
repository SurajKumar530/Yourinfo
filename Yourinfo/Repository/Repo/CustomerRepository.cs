using Dapper;
using System.Data;
using System.Data.SqlClient;
using Yourinfo.Models;
using Yourinfo.Models.Customer;
using Yourinfo.Repository.Interface;
using static Azure.Core.HttpHeader;

namespace Yourinfo.Repository.Repo
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;
        public CustomerRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Gallery
        public List<CustomerGalleryImages> getGalleryData(string customerID)
        {
            try
            {
                CustomerGalleryImages customerView = new CustomerGalleryImages();
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    const string readSp = "sp_GetCustomerGalleryDetails";
                    var dyParam = new DynamicParameters();
                    dyParam.Add("@customerID", customerID, DbType.String, ParameterDirection.Input);
                    var customerList = db.Query<CustomerGalleryImages>(readSp, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                    return customerList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> updateGalleryImages(CustomerGalleryImages images,string oldImage)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            const string insertSP = "sp_UpdateGalleryImages";
                            var dyparam = new DynamicParameters();
                            dyparam.Add("@CustomerID", Convert.ToInt32(images.CustomerId), DbType.Int32, ParameterDirection.Input);
                            dyparam.Add("@ImageName", images.CustomerGalleryName, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@ImagePath", images.CustomerGalleryPath, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@ImageOldName", oldImage, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@IsVerified", null, DbType.Int32, ParameterDirection.Output);

                            // Execute the insert operation
                            await db.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);

                            int errorCode = dyparam.Get<int>("@IsVerified");

                            if (errorCode != 0)
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Commit();
                                return errorCode;
                            }
                            else
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Rollback();
                                return errorCode;
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
        }

        public async Task<int> deleteGalleryImages(string customerID, string oldImage)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            const string insertSP = "[sp_DeleteGalleryImages]";
                            var dyparam = new DynamicParameters();
                            dyparam.Add("@CustomerID", Convert.ToInt32(customerID), DbType.Int32, ParameterDirection.Input);
                            dyparam.Add("@ImageOldName", oldImage, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@IsVerified", null, DbType.Int32, ParameterDirection.Output);

                            // Execute the insert operation
                            await db.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);

                            int errorCode = dyparam.Get<int>("@IsVerified");

                            if (errorCode != 0)
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Commit();
                                return errorCode;
                            }
                            else
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Rollback();
                                return errorCode;
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
        }

        #endregion Gallery

        #region Brochure
        public List<CustomerBrochureImages> getBrochureData(string customerID)
        {
            try
            {
                CustomerBrochureImages customerView = new CustomerBrochureImages();
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    const string readSp = "sp_GetCustomerBrochureDetails";
                    var dyParam = new DynamicParameters();
                    dyParam.Add("@customerID", customerID, DbType.String, ParameterDirection.Input);
                    var customerList = db.Query<CustomerBrochureImages>(readSp, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                    return customerList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> updateBrochureImages(CustomerBrochureImages images, string oldImage)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            const string insertSP = "sp_UpdateBrochureImages";
                            var dyparam = new DynamicParameters();
                            dyparam.Add("@CustomerID", Convert.ToInt32(images.CustomerId), DbType.Int32, ParameterDirection.Input);
                            dyparam.Add("@ImageName", images.CustomerBrochureName, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@ImagePath", images.CustomerBrochurePath, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@ImageOldName", oldImage, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@IsVerified", null, DbType.Int32, ParameterDirection.Output);

                            // Execute the insert operation
                            await db.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);

                            int errorCode = dyparam.Get<int>("@IsVerified");

                            if (errorCode != 0)
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Commit();
                                return errorCode;
                            }
                            else
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Rollback();
                                return errorCode;
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
        }

        public async Task<int> deleteBrochureImages(string customerID, string oldImage)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            const string insertSP = "[sp_DeleteBrochureImages]";
                            var dyparam = new DynamicParameters();
                            dyparam.Add("@CustomerID", Convert.ToInt32(customerID), DbType.Int32, ParameterDirection.Input);
                            dyparam.Add("@ImageOldName", oldImage, DbType.String, ParameterDirection.Input);
                            dyparam.Add("@IsVerified", null, DbType.Int32, ParameterDirection.Output);

                            // Execute the insert operation
                            await db.ExecuteAsync(insertSP, param: dyparam, transaction: transaction, commandType: CommandType.StoredProcedure);

                            int errorCode = dyparam.Get<int>("@IsVerified");

                            if (errorCode != 0)
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Commit();
                                return errorCode;
                            }
                            else
                            {
                                // Commit transaction if all inserts succeed
                                transaction.Rollback();
                                return errorCode;
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
        }

        #endregion Brochure
    }
}

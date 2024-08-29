using Yourinfo.Models.Customer;

namespace Yourinfo.Repository.Interface
{
    public interface ICustomerRepository
    {
        #region Gallery
        List<CustomerGalleryImages> getGalleryData(string customerID);

        Task<int> updateGalleryImages(CustomerGalleryImages images,string oldImage);
        Task<int> deleteGalleryImages(string customerID,string oldImage);
        #endregion Gallery

        #region Brochure
        List<CustomerBrochureImages> getBrochureData(string customerID);

        Task<int> updateBrochureImages(CustomerBrochureImages images, string oldImage);
        Task<int> deleteBrochureImages(string customerID, string oldImage);

        #endregion Brochure
    }
}

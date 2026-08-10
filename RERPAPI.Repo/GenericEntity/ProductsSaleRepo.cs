using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductsSaleRepo : GenericRepo<ProductSale>
    {
        private RTCContext _context = new RTCContext();

        public ProductsSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> CreateAsynC(ProductSale item)
        {
            await _context.ProductSales.AddAsync(item);
            await _context.SaveChangesAsync();
            return item.ID;
        }

        public bool CheckCode(ProductSaleImportExcelDTO item)
        {
            var existing = GetAll(
                x => x.ProductName.Trim().ToLower() == item.ProductName.Trim().ToLower()
                && x.ProductCode.Trim().ToLower() == item.ProductCode.Trim().ToLower()
                && x.IsDeleted == false
                && x.ProductGroupID == item.ProductGroupID);

            var existingApproved = GetAll(
                x => x.ProductCode.Trim().ToLower() == item.ProductCode.Trim().ToLower()
                && x.ProductGroupID != item.ProductGroupID
                && x.IsDeleted != true
                && (x.IsApproved == true || x.IsFix == true)
                && (
                     x.ProductName != item.ProductName ||
                     x.Maker != item.Maker ||
                     x.Unit != item.Unit
                   )
             );

            return existing.Any() || existingApproved.Any();
        }
    }
}
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductsSaleRepo : GenericRepo<ProductSale>
    {
        RTCContext _context = new RTCContext();

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
            var existing = GetAll(x => x.ProductName.Trim().ToLower() == item.ProductName.Trim().ToLower() && x.ProductCode.Trim().ToLower() == item.ProductCode.Trim().ToLower() && x.IsDeleted == false);

            return existing.Any();
        }
    }
}

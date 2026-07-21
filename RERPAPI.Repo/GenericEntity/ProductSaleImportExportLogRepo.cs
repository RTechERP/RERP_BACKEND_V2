using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductSaleImportExportLogRepo : GenericRepo<ProductSaleImportExportLog>
    {

        public ProductSaleImportExportLogRepo(
            CurrentUser currentUser
            ) : base(currentUser)
        {
        }

        public async Task WriteLog(string type, string content, string user)
        {
            await CreateAsync(new ProductSaleImportExportLog
            {
                TypeLog = type,
                ContentLog = content,
                CreatedBy = user,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            });
        }
    }
}
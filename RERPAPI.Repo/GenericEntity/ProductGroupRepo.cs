using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductGroupRepo : GenericRepo<ProductGroup>
    {

        public ProductGroupRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> CreateAsynC(ProductGroup item)
        {
            await CreateAsync(item);
            return item.ID;
        }
    }
}

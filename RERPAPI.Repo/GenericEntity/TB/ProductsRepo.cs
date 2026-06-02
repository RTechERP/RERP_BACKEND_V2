using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.TB
{
    public class ProductsRepo : GenericRepo<Product>
    {
        public ProductsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
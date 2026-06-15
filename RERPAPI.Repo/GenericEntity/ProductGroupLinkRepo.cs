using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductGroupLinkRepo : GenericRepo<ProductGroupLink>
    {
        public ProductGroupLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
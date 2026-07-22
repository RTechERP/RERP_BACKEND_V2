using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductRTCFileRepo : GenericRepo<ProductRTCFile>
    {
        public ProductRTCFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SupplierSaleLinkRepo : GenericRepo<SupplierSaleLink>
    {
        public SupplierSaleLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
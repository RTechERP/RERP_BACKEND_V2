using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SupplierSaleContactRepo : GenericRepo<SupplierSaleContact>
    {
        public SupplierSaleContactRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
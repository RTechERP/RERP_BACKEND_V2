using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SupplierSaleRepo : GenericRepo<SupplierSale>
    {

        public SupplierSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

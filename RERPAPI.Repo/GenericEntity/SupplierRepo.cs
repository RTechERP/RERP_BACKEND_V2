using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class SupplierRepo : GenericRepo<Supplier>
    {
        public SupplierRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
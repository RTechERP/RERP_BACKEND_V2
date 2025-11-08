using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class AddressStockRepo : GenericRepo<AddressStock>
    {
        public AddressStockRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillImportDetailSerialNumberRepo : GenericRepo<BillImportDetailSerialNumber>
    {
        public BillImportDetailSerialNumberRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
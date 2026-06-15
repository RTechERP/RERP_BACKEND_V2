using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillExportDetailSerialNumberRepo : GenericRepo<BillExportDetailSerialNumber>
    {
        public BillExportDetailSerialNumberRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
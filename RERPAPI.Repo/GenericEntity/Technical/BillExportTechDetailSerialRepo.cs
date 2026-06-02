using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillExportTechDetailSerialRepo : GenericRepo<BillExportTechDetailSerial>
    {
        public BillExportTechDetailSerialRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
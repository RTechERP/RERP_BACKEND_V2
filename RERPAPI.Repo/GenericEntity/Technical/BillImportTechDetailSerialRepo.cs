using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillImportTechDetailSerialRepo : GenericRepo<BillImportTechDetailSerial>
    {
        public BillImportTechDetailSerialRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
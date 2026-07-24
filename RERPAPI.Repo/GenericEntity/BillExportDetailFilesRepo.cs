using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillExportDetailFilesRepo : GenericRepo<BillExportDetailFile>
    {
        public BillExportDetailFilesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
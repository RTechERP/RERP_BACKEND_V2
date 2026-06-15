using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportMarketingFileRepo : GenericRepo<DailyReportMarketingFile>
    {
        public DailyReportMarketingFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
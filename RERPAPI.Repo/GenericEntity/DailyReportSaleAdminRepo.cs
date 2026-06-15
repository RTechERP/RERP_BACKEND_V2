using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportSaleAdminRepo : GenericRepo<DailyReportSaleAdmin>
    {
        public DailyReportSaleAdminRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportAccountingRepo : GenericRepo<DailyReportAccounting>
    {
        public DailyReportAccountingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
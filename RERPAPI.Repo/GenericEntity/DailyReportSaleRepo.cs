using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportSaleRepo : GenericRepo<DailyReportSale>
    {
        public DailyReportSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
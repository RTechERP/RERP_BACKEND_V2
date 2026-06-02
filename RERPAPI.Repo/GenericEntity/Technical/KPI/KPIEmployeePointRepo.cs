using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIEmployeePointRepo : GenericRepo<KPIEmployeePoint>
    {
        public KPIEmployeePointRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
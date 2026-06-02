using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIEmployeePointDetailRepo : GenericRepo<KPIEmployeePointDetail>
    {
        public KPIEmployeePointDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIPositionEmployeeRepo : GenericRepo<KPIPositionEmployee>
    {
        public KPIPositionEmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
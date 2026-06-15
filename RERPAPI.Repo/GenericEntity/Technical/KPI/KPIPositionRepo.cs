using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIPositionRepo : GenericRepo<KPIPosition>
    {
        public KPIPositionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
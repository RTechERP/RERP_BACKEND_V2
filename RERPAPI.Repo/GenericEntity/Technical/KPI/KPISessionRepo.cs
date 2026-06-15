using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPISessionRepo : GenericRepo<KPISession>
    {
        public KPISessionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
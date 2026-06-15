using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIEmployeeTeamLinkRepo : GenericRepo<KPIEmployeeTeamLink>
    {
        public KPIEmployeeTeamLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
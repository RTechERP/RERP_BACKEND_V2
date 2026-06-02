using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIEmployeeTeamRepo : GenericRepo<KPIEmployeeTeam>
    {
        public KPIEmployeeTeamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
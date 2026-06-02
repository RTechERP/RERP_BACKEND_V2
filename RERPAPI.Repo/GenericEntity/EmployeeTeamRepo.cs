using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeTeamRepo : GenericRepo<EmployeeTeam>
    {
        public EmployeeTeamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseKPIEmployeeTeamRepo : GenericRepo<CourseKPIEmployeeTeam>
    {
        public CourseKPIEmployeeTeamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
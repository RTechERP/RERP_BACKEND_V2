using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseKPIEmployeeTeamLinkRepo : GenericRepo<CourseKPIEmployeeTeamLink>
    {
        public CourseKPIEmployeeTeamLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
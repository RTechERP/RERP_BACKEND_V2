using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseKPIEmployeeTeamMapRepo : GenericRepo<CourseKPIEmployeeTeamMap>
    {
        public CourseKPIEmployeeTeamMapRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
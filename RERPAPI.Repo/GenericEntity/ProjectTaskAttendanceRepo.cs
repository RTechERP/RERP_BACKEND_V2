using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskAttendanceRepo : GenericRepo<ProjectTaskAttendance>
    {
        public ProjectTaskAttendanceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
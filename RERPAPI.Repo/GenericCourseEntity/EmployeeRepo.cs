using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class EmployeeRepo : GenericCourseRepo<Employee>
    {
        public EmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
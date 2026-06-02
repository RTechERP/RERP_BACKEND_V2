using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseRepo : GenericCourseRepo<Course>
    {
        public CourseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
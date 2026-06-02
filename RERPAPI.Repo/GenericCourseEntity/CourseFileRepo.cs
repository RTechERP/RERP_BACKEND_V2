using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseFileRepo : GenericCourseRepo<CourseFile>
    {
        public CourseFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseAnswersRepo : GenericCourseRepo<CourseAnswer>
    {
        public CourseAnswersRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
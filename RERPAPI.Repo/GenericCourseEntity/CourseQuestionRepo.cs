using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseQuestionRepo : GenericCourseRepo<CourseQuestion>
    {
        public CourseQuestionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseRightAnswersRepo : GenericCourseRepo<CourseRightAnswer>
    {
        public CourseRightAnswersRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
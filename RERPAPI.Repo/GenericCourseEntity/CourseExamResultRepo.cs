using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseExamResultRepo : GenericCourseRepo<RERPAPI.Model.Entities.RTCCourse.CourseExamResult>
    {
        public CourseExamResultRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseExamResultDetailRepo : GenericCourseRepo<CourseExamResultDetail>
    {
        public CourseExamResultDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
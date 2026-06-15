using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseExamEvaluateRepo : GenericCourseRepo<CourseExamEvaluate>
    {
        public CourseExamEvaluateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
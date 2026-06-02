using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseExamPracticeRepo : GenericRepo<CourseExamPractice>
    {
        public CourseExamPracticeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
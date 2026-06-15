using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseExamRepo : GenericCourseRepo<CourseExam>
    {
        public CourseExamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
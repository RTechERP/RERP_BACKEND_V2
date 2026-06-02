using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseLessonHistoryRepo : GenericCourseRepo<CourseLessonHistory>
    {
        public CourseLessonHistoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
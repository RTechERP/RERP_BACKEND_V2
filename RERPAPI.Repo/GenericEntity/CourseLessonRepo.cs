using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseLessonRepo : GenericRepo<CourseLesson>
    {
        public CourseLessonRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
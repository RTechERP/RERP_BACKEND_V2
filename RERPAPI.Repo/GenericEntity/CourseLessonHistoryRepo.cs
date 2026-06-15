using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseLessonHistoryRepo : GenericRepo<CourseLessonHistory>
    {
        public CourseLessonHistoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseAnswerRepo : GenericRepo<CourseAnswer>
    {
        public CourseAnswerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
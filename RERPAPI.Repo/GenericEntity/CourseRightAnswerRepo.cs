using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseRightAnswerRepo : GenericRepo<CourseRightAnswer>
    {
        public CourseRightAnswerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
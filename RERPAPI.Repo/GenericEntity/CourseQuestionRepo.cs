using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseQuestionRepo : GenericRepo<CourseQuestion>
    {
        public CourseQuestionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
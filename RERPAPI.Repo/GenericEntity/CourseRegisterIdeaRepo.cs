using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseRegisterIdeaRepo : GenericRepo<CourseRegisterIdea>
    {
        public CourseRegisterIdeaRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
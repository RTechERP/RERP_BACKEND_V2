using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseRepo : GenericRepo<Course>
    {
        public CourseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
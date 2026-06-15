using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseFilesRepo : GenericRepo<CourseFile>
    {
        public CourseFilesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
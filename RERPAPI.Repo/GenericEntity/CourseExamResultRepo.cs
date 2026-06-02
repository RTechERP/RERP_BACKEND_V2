using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseExamResultRepo : GenericRepo<CourseExamResult>
    {
        public CourseExamResultRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseExamResultDetailRepo : GenericRepo<CourseExamResultDetail>
    {
        public CourseExamResultDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
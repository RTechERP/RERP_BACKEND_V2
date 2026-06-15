using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseExamEvaluateRepo : GenericRepo<CourseExamEvaluate>
    {
        public CourseExamEvaluateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
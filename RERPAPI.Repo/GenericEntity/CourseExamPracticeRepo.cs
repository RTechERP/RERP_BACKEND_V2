using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CourseExamPracticeRepo : GenericRepo<CourseExamPractice>
    {
        public CourseExamPracticeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
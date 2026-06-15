using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectSurveyDetailRepo : GenericRepo<ProjectSurveyDetail>
    {
        public ProjectSurveyDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
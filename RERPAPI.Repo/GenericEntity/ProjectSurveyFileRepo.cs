using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectSurveyFileRepo : GenericRepo<ProjectSurveyFile>
    {
        public ProjectSurveyFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
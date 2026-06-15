using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectSurveyDTO
    {
        public ProjectSurvey projectSurvey { get; set; }
        public List<ProjectSurveyDetail> projectSurveyDetails { get; set; }
        public List<ProjectSurveyFile>? projectSurveyFiles { get; set; }
        public List<int> deletedFiles { get; set; }
    }
}
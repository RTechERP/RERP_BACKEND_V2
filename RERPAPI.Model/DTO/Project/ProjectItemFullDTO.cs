using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Project
{
    public class ProjectItemFullDTO
    {
        public ProjectItemProblem? projectItemProblem { get; set; }
        public List<ProjectItem>? projectItems { get; set; }
        public ProjectItemFile? ProjectItemFile { get; set; }
    }
}
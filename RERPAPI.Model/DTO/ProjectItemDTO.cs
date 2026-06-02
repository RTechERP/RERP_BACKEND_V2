using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectItemDTO
    {
        public List<ProjectItem> projectItem { get; set; }
        public int ProjectID { get; set; }
        public List<int>? DeletedIdsprojectItem { get; set; }
    }
}
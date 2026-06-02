using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectRequestDTO : ProjectRequest
    {
        public List<ProjectRequestFile>? projectRequestFile { get; set; }
        public List<int>? deletedFileID { get; set; }
    }
}
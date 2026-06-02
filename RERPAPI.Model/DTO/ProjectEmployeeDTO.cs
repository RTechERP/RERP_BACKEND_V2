using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectEmployeeDTO
    {
        public List<int> deletedIds { get; set; }
        public int ProjectID { get; set; }
        public List<ProjectEmployee> prjEms { get; set; }
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectTypeDTO : ProjectType
    {
        public int? ProjectTypeDepartmentID { get; set; }
    }
}
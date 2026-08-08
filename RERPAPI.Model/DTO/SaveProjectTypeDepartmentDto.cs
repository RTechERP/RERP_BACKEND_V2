using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class SaveProjectTypeDepartmentDto
    {
        public int DepartmentID { get; set; }
        public List<int> ProjectTypeIDs { get; set; } = new List<int>();
    }
}

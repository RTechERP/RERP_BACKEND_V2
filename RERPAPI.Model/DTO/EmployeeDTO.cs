using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class EmployeeDTO : Employee
    {
        public string DepartmentName { get; set; }
        public string ChucVu { get; set; }
    }
}
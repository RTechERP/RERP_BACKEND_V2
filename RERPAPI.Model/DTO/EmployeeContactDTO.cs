using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class EmployeeContactDTO : Employee
    {
        public Int64 STT { get; set; }
        public string DepartmentName { get; set; }
        public string ChucVuHD { get; set; }
        public string ChucVu { get; set; }
        public string EmployeeTeamName { get; set; }
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class EmployeeOverTimeDTO
    {
        public List<EmployeeOvertime> EmployeeOvertimes { get; set; }
        public EmployeeOvertimeFile? employeeOvertimeFile { get; set; }
    }
}
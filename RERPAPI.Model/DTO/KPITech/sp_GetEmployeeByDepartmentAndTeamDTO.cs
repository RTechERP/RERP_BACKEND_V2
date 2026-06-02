namespace RERPAPI.Model.DTO.KPITech
{
    public class sp_GetEmployeeByDepartmentAndTeamDTO
    {
        public int? EmployeeID { get; set; } = 0;
        public int? KPIEmployeeTeamID { get; set; }
        public int? DepartmentID { get; set; }
        public int? TeamID { get; set; }
        public int? YearValue { get; set; }
        public int? QuarterValue { get; set; }
    }
}
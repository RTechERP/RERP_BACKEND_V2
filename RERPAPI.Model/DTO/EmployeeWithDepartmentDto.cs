namespace RERPAPI.Model.DTO
{
    public class EmployeeWithDepartmentDto
    {
        public int ID { get; set; }
        public string? FullName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
    }
}
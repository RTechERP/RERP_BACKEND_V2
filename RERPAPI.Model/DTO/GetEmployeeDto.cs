namespace RERPAPI.Model.DTO
{
    public class GetEmployeeDto
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
    }
}
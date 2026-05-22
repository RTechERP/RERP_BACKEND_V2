namespace RERPAPI.Model.DTO
{
    public class EmployeeAttendanceNewDTO
    {
        public DateTime time { get; set; }
        public string? employeeNoString { get; set; } // ID nhân viên dạng string, có thể là mã nhân viên hoặc tên nhân viên
        public string? name { get; set; } // Tên nhân viên
    }
}
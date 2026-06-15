using RERPAPI.Model.Common;

namespace RERPAPI.Model.Param
{
    public class JobRequirementParam
    {
        public DateTime DateStart { get; set; } = TextUtils.MinDate;
        public DateTime DateEnd { get; set; } = TextUtils.MaxDate;
        public string Request { get; set; } = string.Empty;
        public int EmployeeID { get; set; } = 0;
        public int Step { get; set; } = 0;
        public int DepartmentID { get; set; } = 0;
        public int ApprovedTBPID { get; set; } = 0;
    }
}
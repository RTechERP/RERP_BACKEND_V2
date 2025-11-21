using System;

namespace RERPAPI.Model.Param.HRM
{
    public class EmployeeWFHRequestParam
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Keyword { get; set; } = "";
        public int DepartmentId { get; set; } = 0;
        public int IdApprovedTP { get; set; } = 0;
        public int Status { get; set; } = -1;
    }
}

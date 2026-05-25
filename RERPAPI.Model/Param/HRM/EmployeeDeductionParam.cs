using System;

namespace RERPAPI.Model.Param.HRM
{
    public class EmployeeDeductionParam
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public string Keyword { get; set; } = "";
        public int DeductionType { get; set; }
        public int IsOverride { get; set; } = 0;
    }
}

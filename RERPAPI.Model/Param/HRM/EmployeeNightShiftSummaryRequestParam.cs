using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM
{
    public class EmployeeNightShiftSummaryRequestParam
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public string? KeyWord { get; set; }
      
    }
}

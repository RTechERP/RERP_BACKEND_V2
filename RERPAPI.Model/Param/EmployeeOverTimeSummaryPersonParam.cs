using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeOverTimeSummaryPersonParam
    {
        public int? Page { get; set; } = 1;
        public int? Size { get; set; } = 500000;
        public int? DepartmentID { get; set; } = 0;
        public int? EmployeeID { get; set; } = 0;
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? FilterText { get; set; }
        public int? IDApprovedTP { get; set; }
        public int? TeamID { get; set; }
        public int? Status { get; set; }
    }
}

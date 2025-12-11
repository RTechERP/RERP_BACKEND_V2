using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM
{
    public class EmployeeNoFingerPrintRequestParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int Status { get; set; }
        public int IDApprovedTP { get; set; }
        public string? KeyWord { get; set; }
        public int? DepartmentID { get; set; }
        public int? EmployeeID { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
    }
}

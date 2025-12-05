using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EmployeeOnLeavePersonParam
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 1000000;
        public int? DepartmentID { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? Keyword { get; set; }
        public int IDApprovedTP { get; set; }
        public int Status { get; set; }


    }
}

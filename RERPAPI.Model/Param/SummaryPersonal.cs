using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class SummaryPersonal
    {
        public int? DepartmentID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? Keyword { get; set; }
        public int IsApproved { get; set; }
        public int? Type { get; set; }
    }
}

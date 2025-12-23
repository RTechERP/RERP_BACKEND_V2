using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class SummaryJobrequirementRequestParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? request { get; set; }
        public int? EmployeeID { get; set; }
      
        public int? Step { get; set; }
        public int DepartmentID { get; set; }
    }
}

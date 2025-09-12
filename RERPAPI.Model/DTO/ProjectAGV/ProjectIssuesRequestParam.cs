using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectIssuesRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? ProjectID { get; set; }
        public int? DepartmentID { get; set; }
        public int? EmployeeID { get; set; }
        public string? Keyword { get; set; }
    }
}

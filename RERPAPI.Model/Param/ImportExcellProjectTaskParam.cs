using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class ImportExcellProjectTaskParam
    {
        public string TT { get; set; }
        public string Title { get; set; }
        public string ProjectCode { get; set; }
        public string EmployeeCode { get; set; }
        public string AssigneesCodes { get; set; }
        public string RelatedCodes { get; set; }
        public int TaskComplexity { get; set; } = 1;
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }
    }
}

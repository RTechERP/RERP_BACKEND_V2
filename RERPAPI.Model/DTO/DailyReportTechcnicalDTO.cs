using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DailyReportTechcnicalDTO
    {
        public string FullName { get; set; }
        public string PositionName { get; set; }
        public DateTime? DateReport { get; set; }
        public string Content { get; set; }
        public string Results { get; set; }
        public string PlanNextDay { get; set; }
        public string Backlog { get; set; }
        public string Problem { get; set; }
        public string ProblemSolve { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

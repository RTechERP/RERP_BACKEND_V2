using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectDailyReportDto
    {
        public int ProjectItemID { get; set; }
        public int TotalHours {  get; set; }
        public int TotalHourOT { get; set; }
        public decimal PercentComplete { get; set; }
        public string ContentReport { get; set; }
        public string? PlanNextDay { get; set; } = "";
        public string? Problem { get; set; } = "";
        public string? ProblemSolve { get; set; } = "";
        public string? Backlog { get; set; } = "";
        public string? Note { get; set; } = "";
    }
}

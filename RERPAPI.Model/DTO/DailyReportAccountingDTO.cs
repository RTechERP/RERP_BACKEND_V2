using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DailyReportAccountingDTO
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public DateTime ReportDate { get; set; }
        public string? Content { get; set; }
        public string? Result { get; set; }
        public string? NextPlan { get; set; }
        public string? PendingIssues { get; set; }
        public string? Urgent { get; set; }
        public string? MistakeOrViolation { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class WorkPlanSummaryParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? Keyword { get; set; }
        public int DepartmentId { get; set; }
        public int TeamId { get; set; }
        public int Type { get; set; } // 0: Tổng hợp, 1: Chi tiết
        public int UserId { get; set; }
    }
}

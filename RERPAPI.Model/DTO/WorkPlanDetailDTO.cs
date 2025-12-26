using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class WorkPlanDetailDTO
    {
        public string FullName { get; set; }
        public string Day1 { get; set; }
        public string Day2 { get; set; }
        public string Day3 { get; set; }
        public string Day4 { get; set; }
        public string Day5 { get; set; }
        public string Day6 { get; set; }
        public string Day7 { get; set; }
        public string WorkPlanPreviousWeek { get; set; }
        public string WorkPlanCurrentWeek { get; set; }

        public class WorkPlanPreCur
        {
            public int UserID { get; set; }
            public string WorkContentPre { get; set; }
            public string WorkContentCur { get; set; }
        }
    }
}

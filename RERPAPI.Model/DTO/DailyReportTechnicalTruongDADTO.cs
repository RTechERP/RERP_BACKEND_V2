using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DailyReportTechnicalTruongDADTO
    {
        public int UserID { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public bool Confirm { get; set; }
        public int ID { get; set; }
        public DateTime DateReport { get; set; }

        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectText { get; set; }

        public decimal TotalHours { get; set; }

        public string Content { get; set; }
        public string Results { get; set; }
        public string PlanNextDay { get; set; }
        public string Backlog { get; set; }
        public string Problem { get; set; }
        public string ProblemSolve { get; set; }
        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Type { get; set; }
        public string TypeText { get; set; }
        public string PositionName { get; set; }

        public string ProjectItemCode { get; set; }
        public string ProjectItemMission { get; set; }

        public DateTime ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }

        public decimal PercentageActual { get; set; }
        public int TypeHoliday { get; set; }
    }
}

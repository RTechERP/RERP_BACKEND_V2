using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.KPITech
{
    public class spGetEmployeeRulePointByKPIEmpPointIDNewResultDTO
    {
        public int? ID { get; set; }
        public string? STT { get; set; }
        public int? ParentID { get; set; }
        public string? RuleContent { get; set; }
        public string? FormulaCode { get; set; } 
        public decimal? MaxPercentageAdjustment { get; set; }
        public decimal? MaxPercent { get; set; }
        public decimal? PercentageAdjustment { get; set; }
        public string? RuleNote { get; set; }
        public string? Note { get; set; }
        public int? EmpPointDetailID { get; set; }
        public int? KPIEmployeePointID { get; set; }
        public int? KPIEvaluationRuleDetailID { get; set; }
        public decimal? PercentBonus { get; set; }
        public decimal? PercentRemaining { get; set; }
        public string? EvaluationCode { get; set; }
        public decimal? FirstMonth { get; set; }
        public decimal? SecondMonth { get; set; }
        public decimal? ThirdMonth { get; set; }
        public decimal? TotalError { get; set; }
    }
}

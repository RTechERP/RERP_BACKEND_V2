using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.KPITech
{
    public class SaveKPIEmployeePointDetailRequest
    {
        public int KPISessionID { get; set; }
        public int EmployeeID { get; set; }
        public decimal? PercentRemaining { get; set; }
        public int KPIEmployeePointID { get; set; }
        public int KPIEvaluationRuleID { get; set; }
        public List<KPIEmployeePointDetailParam> lstKPIEmployeePointDetail { get; set; } = new List<KPIEmployeePointDetailParam>();
    }
    public class KPIEmployeePointDetailParam
    {
        public int? EmpPointDetailID { get; set; }
        public int ID { get; set; }
        public decimal? FirstMonth { get; set; }
        public decimal? SecondMonth { get; set; }
        public decimal? ThirdMonth { get; set; }
        public decimal? PercentBonus { get; set; }
        public decimal? PercentRemaining { get; set; }
    }
}

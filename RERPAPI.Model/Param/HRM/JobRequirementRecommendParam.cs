using RERPAPI.Model.Common;
using System;

namespace RERPAPI.Model.Param.HRM
{
    public class JobRequirementRecommendParam
    {
        public int? ID { get; set; }
        public string Keyword { get; set; } = "";
        public DateTime DateStart { get; set; } = TextUtils.MinDate;
        public DateTime DateEnd { get; set; } = TextUtils.MaxDate;
        public int? EmployeeID { get; set; }
    }
}

using RERPAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class WorkPlanParam
    {
        public DateTime? DateStart { get; set; } = TextUtils.MinDate;
        public DateTime? DateEnd { get; set; } = TextUtils.MaxDate;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string Keyword { get; set; } = string.Empty;
    }
}

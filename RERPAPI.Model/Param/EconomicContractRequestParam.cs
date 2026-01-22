using RERPAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class EconomicContractRequestParam
    {
        public DateTime? DateStart { get; set; } = TextUtils.MinDate;
        public DateTime? DateEnd { get; set; } = TextUtils.MaxDate;
        public string? Keyword { get; set; } = "";
        public int? TypeNCC { get; set;} = 0;
        public int? Type { get; set;} = 0;
    }
}

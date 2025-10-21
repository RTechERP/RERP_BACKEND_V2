using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Asset
{
    public class AssetRecoveryRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        
        public int? EmployeeReturnID { get; set; }
        public int? EmployeeRecoveryID { get; set; }
        public int? Status { get; set; } = -1;
        public string? Filtertext { get; set; } = "";   
        public int PageSize { get; set; } = 1;
        public int PageNumber { get; set; } = 10000;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Asset
{
    public class AssetmanagementRequestParam
    {
        public string? FilterText { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? Status { get; set; }
        public string? Department { get; set; }
    }
}


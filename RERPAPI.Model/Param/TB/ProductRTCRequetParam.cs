using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.TB
{
    public class ProductRTCRequetParam
    {
        public int? ProductGroupID { get; set; }
        public string? Keyword { get; set; }
        public int? CheckAll { get; set; }
        public int? WarehouseID { get; set; }
        public int?ProductRTCID { get; set; }
        public string? ProductGroupNo { get; set; }

        public int Page { get; set; } = 10000;
        public int Size { get; set; } = 1;
        public int WarehouseType { get; set; } = 1;
    }
}

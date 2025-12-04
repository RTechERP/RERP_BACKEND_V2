using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Technical
{
    public class InventoryDemoRequestParam
    {
        public int? ProductGroupID { get; set; }
        public string? Keyword { get; set; }
        public int? CheckAll { get; set; }
        public int? WarehouseID { get; set; }
        public int? ProductRTCID { get; set; }
        public int? WarehouseType { get; set; }

    }
}

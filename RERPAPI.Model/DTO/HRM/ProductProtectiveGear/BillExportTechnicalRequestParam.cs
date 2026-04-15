using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM.ProductProtectiveGear
{
    public class BillExportTechnicalRequestParam
    {
        public int? Page { get; set; }
        public int? Size { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? Status { get; set; }
        public string? FilterText { get; set; }
        public int? WarehouseID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.Technical
{
    public class GetImportExportTechnicalRequestParam
    {
        public int? Page { get; set; }
        public int? Size { get; set; }
        public string? FilterText { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? Status { get; set; }
      
        public int? WarehouseID { get; set; }
        public int? BillType { get; set; }
        public int? ReceiverID { get; set; }
        public int WarehouseType { get; set; } = 1;
    }
}

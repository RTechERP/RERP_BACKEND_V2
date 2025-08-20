using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportTechnicalFullDTO
    {
        public BillExportTechnical? billExportTechnical { get; set; }
        public HistoryDeleteBill? historyDeleteBill { get; set; }
        public List<BillExportDetailTechnical>? billExportDetailTechnicals { get; set; }
        public List<BillExportTechDetailSerial>? billExportTechDetailSerials { get; set; }
        public List<BillExportTechnicalLog>? billExportTechnicalLog { get; set; }
        public List<InventoryDemo>? inentoryDemos { get; set; }
        public List<HistoryProductRTC>? historyProductRTCs { get; set; }
    }
}

using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillimporttechnicalFullDTO
    {
        public BillImportTechnical? billImportTechnical { get; set; }
        public HistoryDeleteBill? historyDeleteBill { get; set; }
        public List<BillImportDetailTechnical>? billImportDetailTechnicals { get; set; }
        public List<BillImportTechDetailSerial>? billImportTechDetailSerials { get; set; }
        public List<BillDocumentImportTechnical>? documentImportPONCCs { get; set; }
        public int? PonccID { get; set; }
    }
}
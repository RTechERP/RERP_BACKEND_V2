using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillImportDTO
    {
        public BillImport? billImport { get; set; }
        public List<BillImportDetailDTO>? billImportDetail { get; set; }
        public List<int>? DeletedDetailIDs { get; set; }
        public List<DocumentImportPONCC>? billDocumentImports { get; set; }
    }
}

using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillDocumentImportDTO
    {
        public BillDocumentImport BillDocuments { get; set; }
        public string? lydo { get; set; }
        public string? note { get; set; }
    }
}
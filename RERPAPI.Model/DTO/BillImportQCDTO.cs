using Microsoft.AspNetCore.Http;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillImportQCDTO
    {
        public BillImportQC? billImportQC { get; set; }
        public List<BillImportQCDetail> billImportQCDetails { get; set; }
        public List<int>? DeletedDetailIds { get; set; }
        public List<int>? DeletedCheckSheetFileIds { get; set; }
        public List<int>? DeletedReportFileIds { get; set; }
        public List<BillImportQCFileDTO>? CheckSheetFiles { get; set; }
        public List<BillImportQCFileDTO>? ReportFiles { get; set; }
    }

    public class BillImportQCFileDTO
    {
        public IFormFile? File { get; set; }
        public string? FileName { get; set; }
        public int BillImportQCDetailId { get; set; }
        public int FileType { get; set; }
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
    }
}

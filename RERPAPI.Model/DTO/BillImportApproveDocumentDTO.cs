using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillImportApproveDocumentDTO : BillImport
    {
        public int? DoccumentReceiverID { get; set; }
    }
}

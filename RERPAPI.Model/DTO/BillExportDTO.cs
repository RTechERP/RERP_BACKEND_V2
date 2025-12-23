
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillExportDTO
    {
        public BillExport? billExport { get; set; }
        public List<BillExportDetailExtendedDTO>? billExportDetail { get; set; }
        public List<int>? DeletedDetailIDs { get; set; }
        public List<int>? POKHDetailIDs { get; set; }
    }
}

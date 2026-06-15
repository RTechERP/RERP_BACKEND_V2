using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class BillExportTechnicalProtectiveGearDTO
    {
        public BillExportTechnical BillExportTechnical { get; set; }
        public List<BillExportDetailTechnical>? BillExportDetailTechnical { get; set; }
        public List<int>? DeletedDetailIds { get; set; }
    }
}
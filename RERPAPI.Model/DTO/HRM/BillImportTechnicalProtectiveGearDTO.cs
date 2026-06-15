using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class BillImportTechnicalProtectiveGearDTO
    {
        public BillImportTechnical BillImportTechnical { get; set; }
        public List<BillImportDetailTechnical>? BillImportDetailTechnical { get; set; }
        public List<int>? DeletedDetailIds { get; set; }
    }
}
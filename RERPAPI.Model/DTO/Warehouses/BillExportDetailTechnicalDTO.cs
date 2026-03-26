using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Warehouses
{
    public class BillExportDetailTechnicalDTO : BillExportDetailTechnical
    {
        public List<BillExportTechDetailSerialDTO>? billExportTechDetailSerials { get; set; }

    }
}

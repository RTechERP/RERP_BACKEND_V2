using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Warehouses.AGV
{
    public class AGVBillExportDTO : AGVBillExport
    {
        public List<AGVBillExportDetail> AGVBillExportDetails { get; set; } = new List<AGVBillExportDetail>();
    }
}
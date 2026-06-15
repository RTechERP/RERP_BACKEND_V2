using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Warehouses.AGV
{
    public class AGVBillImportDTO : AGVBillImport
    {
        public List<AGVBillImportDetail> AGVBillImportDetails { get; set; } = new List<AGVBillImportDetail>();
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.TB
{
    public class ProductRTCImportExcelDTO : ProductRTC
    {
        public string? ProductGroupName { get; set; }
        public int WarehouseType { get; set; }
        public string? LocationName { get; set; }
        public string? LocationCode { get; set; }
    }
}

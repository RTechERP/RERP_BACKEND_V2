using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProductSaleImportExcelDTO : ProductSale
    {
        public string? ProductGroupName { get; set; }
        public string? FirmName { get; set; }
        public string? UnitName { get; set; }
        public string? LocationName { get; set; }
        public string? ProductGroupNo { get; set; }

    }
}

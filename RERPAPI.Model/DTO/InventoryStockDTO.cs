namespace RERPAPI.Model.DTO
{
    public class InventoryStockDTO
    {
        public string? ProductSaleCode { get; set; }
        public string? ProductSaleName { get; set; }
        public decimal? Quantity { get; set; }
        public string? Note { get; set; }
    }
}
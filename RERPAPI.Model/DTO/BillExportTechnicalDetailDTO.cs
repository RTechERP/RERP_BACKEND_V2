using System;

namespace RERPAPI.Model.DTO
{
    public class BillExportTechnicalDetailDTO
    {
        public int? ID { get; set; }
        public int? STT { get; set; }
        public int? BillExportTechID { get; set; }
        public int? UnitID { get; set; }
        public string? UnitName { get; set; }
        public int? ProjectID { get; set; }
        public int? ProductID { get; set; }
        public int? Quantity { get; set; }
        public int? TotalQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Note { get; set; }
        public string? Internalcode { get; set; }
        public int? HistoryProductRTCID { get; set; }
        public int? ProductRTCQRCodeID { get; set; }
        public int? WarehouseID { get; set; }
        public int? BillImportDetailTechnicalID { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? WarehouseType { get; set; }
        public string? Note1 { get; set; }
        public string? ProductCodeRTC { get; set; }
        public string? UnitName1 { get; set; }
        public int? NumberInStore { get; set; }
        public int? Number { get; set; }
        public int? ProductRTCQRCodeID1 { get; set; }
        public string? ProductQRCode { get; set; }
        public string? Maker { get; set; }
    }
}

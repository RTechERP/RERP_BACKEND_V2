using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class ProductsSaleImportExportDTO
    {
        public int? WarehouseId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? RequestDate { get; set; }

        public int? UserId { get; set; }

        public string? Note { get; set; }

        public int? ProductGroupId { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }

        public int? RulePayId { get; set; }

        public int? SenderExportId { get; set; }
        public int? ReciverExportId { get; set; }

        public int? DeliverImportId { get; set; }
        public int? ReciverImportId { get; set; }

        public string? DeliverImportText { get; set; }
        public string? ReciverImportText { get; set; }
        public string? SenderExportText { get; set; }
        public string? ReciverExportText { get; set; }

        public string? ProductGroupText { get; set; }

        public List<ProductsSaleImportExportDetailDTO> DataDetails { get; set; } = new();
    }

    public class ProductsSaleImportExportDetailDTO
    {
        public int? Stt { get; set; }

        public int? ExportProductId { get; set; }
        public string? ExportProductNewCode { get; set; }
        public string? ExportProductCode { get; set; }
        public string? ExportProductName { get; set; }
        public decimal? ExportStockQty { get; set; }

        public int? ImportProductId { get; set; }
        public string? ImportProductNewCode { get; set; }
        public string? ImportProductCode { get; set; }
        public string? ImportProductName { get; set; }

        public decimal? Quantity { get; set; }

        public string? Note { get; set; }
    }
}
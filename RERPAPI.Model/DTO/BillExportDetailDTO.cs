using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class BillExportDetailDTO
    {
        public BillExportDetail billExportDetail { get; set; } = new BillExportDetail();
        public List<BillExportDetailSerialNumber> SerialNumbers { get; set; } = new List<BillExportDetailSerialNumber>();
    }


    public class BillExportDetailExtendedDTO : BillExportDetail
    {
        public string? ChosenInventoryProject { get; set; }
        // ⭐ CẦN THÊM các trường sau:

        /// <summary>
        /// Mã sản phẩm (để hiển thị trong validation message)
        /// </summary>
        public string? ProductCode { get; set; }

        /// <summary>
        /// Mã sản phẩm mới (NewCode)
        /// </summary>
        public string? ProductNewCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// POKHDetailIDActual (frontend đang dùng field này thay vì POKHDetailID)
        /// </summary>
        public int? POKHDetailIDActual { get; set; }

        /// <summary>
        /// Số PO (Purchase Order Number)
        /// </summary>
        public string? PONumber { get; set; }

        /// <summary>
        /// Unit/Đơn vị tính (để skip validate với đơn vị "m", "mét")
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Đơn vị tính (tên đầy đủ)
        /// </summary>
        public string? UnitName { get; set; }

        /// <summary>
        /// ProductCodeExport (danh sách mã sản phẩm từ kho giữ)
        /// Frontend có dùng field này
        /// </summary>
        public string? ProductCodeExport { get; set; }

        /// <summary>
        /// ChildID (để tracking row trong logic phân bổ kho giữ)
        /// Frontend dùng để phân biệt các row
        /// </summary>
        public int? ChildID { get; set; }

        /// <summary>
        /// ImportDetailID (mapping với BillImportDetailID)
        /// Frontend có map field này
        /// </summary>
        public int? ImportDetailID { get; set; }
        public bool? ForceReallocate { get; set; }
    }
}

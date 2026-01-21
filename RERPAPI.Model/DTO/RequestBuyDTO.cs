namespace RERPAPI.Model.DTO
{
    public class RequestBuyDTO
    {

        /// <summary>
        /// ID JobRequirement, nếu là VPP sẽ override trong API
        /// </summary>
        public int JobRequirementID { get; set; }

        /// <summary>
        /// Deadline yêu cầu mua
        /// </summary>
        public DateTime Deadline { get; set; }

        /// <summary>
        /// ID người tạo request
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// Danh sách sản phẩm cần yêu cầu mua
        /// </summary>
        public List<RequestBuyProductDTO> Products { get; set; } = new List<RequestBuyProductDTO>();

        /// <summary>
        /// Nếu là VPP (vật phẩm văn phòng) thì JobRequirementID sẽ override = 999999
        /// </summary>
        public bool IsVPP { get; set; } = false;

        /// <summary>
        /// ProjectPartlistPriceRequestTypeID
        /// </summary>
        public int ProjectPartlistPriceRequestTypeID { get; set; } = 0;
    }

    public class RequestBuyProductDTO
    {

        public int ID { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string ProductCode { get; set; } = "";

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; } = "";

        /// <summary>
        /// Số lượng yêu cầu mua
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Ghi chú từ HR
        /// </summary>
        public string NoteHR { get; set; } = "";

        /// <summary>
        /// Tên đơn vị tính (UnitName)
        /// </summary>
        public string UnitName { get; set; } = "";
        public string Maker { get; set; } = "";
        public int? SupplierSaleID { get; set; }
    }

}

namespace RERPAPI.Model.DTO
{
    public class BillExportTechnicalDTO
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public int? BillType { get; set; }
        public string? CustomerName { get; set; }
        public int? CustomerID { get; set; }
        public string? Receiver { get; set; }
        public string? Deliver { get; set; }
        public string? Addres { get; set; }
        public int? Status { get; set; }
        public string? WarehouseType { get; set; }
        public string? Note { get; set; }
        public string? Image { get; set; }
        public int? ReceiverID { get; set; }
        public int? DeliverID { get; set; }
        public int? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public bool? CheckAddHistoryProductRTC { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? ProjectName { get; set; }
        public int? WarehouseID { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? SupplierSaleID { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerShortName { get; set; }
        public string? CodeNCC { get; set; }
        public bool? StatusBIT { get; set; }
        public int RowNumber { get; set; }
        public string? BillTypeText { get; set; }
        public DateTime? DateStatus { get; set; }
        public string? NameNCC { get; set; }
        public string? EmployeeCode { get; set; }
        public string? DepartmentName { get; set; }
        public int? ApproverID { get; set; }
        public string? EmployeeApproveName { get; set; }
    }
}

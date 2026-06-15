namespace RERPAPI.Model.DTO
{
    public class BillExportRQPDTO
    {
        public string Code { get; set; }
        public int Status { get; set; }
        public int SenderID { get; set; }
        public int SenderEmployeeID { get; set; }
        public int UserID { get; set; }
        public int WarehouseID { get; set; }
        public DateTime RequestDate { get; set; }
        public int CustomerID { get; set; }
        public string Address { get; set; }
        public int KhoTypeID { get; set; }
        public string GroupID { get; set; }
        public string WarehouseType { get; set; }
        public string WarehouseCode { get; set; }
        public bool IsPOKH { get; set; }
    }
}
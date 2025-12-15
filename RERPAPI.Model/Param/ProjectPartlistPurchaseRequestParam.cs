namespace RERPAPI.Model.Param
{
    public class ProjectPartlistPurchaseRequestParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// 0:Tất cả;1:Yêu cầu báo giá;2:Đã check giá;3:Đã hoàn thành
        /// </summary>
        public int StatusRequest { get; set; }
        public int ProjectID { get; set; }
        public string? Keyword { get; set; }
        public int SupplierSaleID { get; set; }
        public int IsApprovedTBP { get; set; }
        public int IsApprovedBGD { get; set; }
        public int IsCommercialProduct { get; set; } = -1;
        public int POKHID { get; set; }
        public int ProductRTCID { get; set; }
        public int IsDeleted { get; set; }
        public int IsTechBought { get; set; }
        public int IsJobRequirement { get; set; }
        public int? EmployeeID { get; set; }
    }
}

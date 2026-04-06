using System;

namespace RERPAPI.Model.DTO
{
    // Generated from procedure: spGetProductGroupWarehouse
    public class spGetProductGroupWarehouseResultDTO
    {
        public int? ID { get; set; }
        public int? ProductGroupID { get; set; }
        public int? WarehouseID { get; set; }
        public int? EmployeeID { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
        public string? FullName { get; set; }
        public int? UserID { get; set; }
    }
}

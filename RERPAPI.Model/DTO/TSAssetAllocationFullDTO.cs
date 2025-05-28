public class TSAssetAllocationFullDTO
{
    // TSAssetAllocation (Master table)
    public int ID { get; set; }
    public string Code { get; set; }
    public DateTime DateAllocation { get; set; }
    public int EmployeeID { get; set; }
    public string Note { get; set; }
    public int Status { get; set; }

    // Chi tiết phân bổ tài sản
    public List<TSAssetAllocationDetailFullDTO> AssetDetails { get; set; } = new();
}

public class TSAssetAllocationDetailFullDTO
{
    // TSAssetAllocationDetail
    public int ID { get; set; }
    public int STT { get; set; }
    public int AssetManagementID { get; set; }
    public int Quantity { get; set; }
    public string Note { get; set; }
    public int EmployeeID { get; set; }

    // Dùng để cập nhật TSAssetManagement
    public int DepartmentID { get; set; }
    public string UpdatedBy { get; set; }

}

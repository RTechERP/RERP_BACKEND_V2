namespace RERPAPI.Model.Entities;

public partial class AssetAllocationLog
{
    public int ID { get; set; }

    public int? AssetAllocationID { get; set; }

    public string? ContentLog { get; set; }

    public string? TypeLog { get; set; }

    public int? EmployeeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
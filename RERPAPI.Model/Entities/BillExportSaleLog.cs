namespace RERPAPI.Model.Entities;

public partial class BillExportSaleLog
{
    public int ID { get; set; }

    public int? BillExportID { get; set; }

    public string? TypeLog { get; set; }

    public string? ContentLog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
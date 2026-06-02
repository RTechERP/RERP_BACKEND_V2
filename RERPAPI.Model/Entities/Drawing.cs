namespace RERPAPI.Model.Entities;

public partial class Drawing
{
    public int ID { get; set; }

    public string? DrawingName { get; set; }

    public string? Version { get; set; }

    public string? ServerPath { get; set; }

    public int? ProjectID { get; set; }

    public int? DesignByID { get; set; }

    public DateTime? DesignDate { get; set; }

    public int? CheckedByID { get; set; }

    public DateTime? CheckedDate { get; set; }

    public int? ApprovedByID { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectTypeID { get; set; }
}
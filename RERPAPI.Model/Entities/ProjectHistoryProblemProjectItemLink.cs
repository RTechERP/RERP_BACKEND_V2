namespace RERPAPI.Model.Entities;

public partial class ProjectHistoryProblemProjectItemLink
{
    public int ID { get; set; }

    public int? ProjectHistoryProblemID { get; set; }

    public int? ProjectItemID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
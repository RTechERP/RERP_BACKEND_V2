namespace RERPAPI.Model.Entities;

public partial class PollSection
{
    public int ID { get; set; }

    public int? PollFormID { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? SortOrder { get; set; }

    public string? ShowIfJson { get; set; }

    public string? BranchingRulesJson { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
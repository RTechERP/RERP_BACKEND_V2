namespace RERPAPI.Model.Entities;

public partial class PollQuestionOption
{
    public int ID { get; set; }

    public int? PollQuestionID { get; set; }

    public string? OptionText { get; set; }

    public string? OptionValue { get; set; }

    public int? SortOrder { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
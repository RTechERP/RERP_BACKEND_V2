namespace RERPAPI.Model.Entities;

public partial class PollForm
{
    public int ID { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsPublic { get; set; }

    public bool? IsDeleted { get; set; }

    public string? BackgroundImagePath { get; set; }

    public string? TitleColor { get; set; }

    public bool? IsNotifycation { get; set; }
}
namespace RERPAPI.Model.Entities;

public partial class PollResponse
{
    public int ID { get; set; }

    public int? PollFormID { get; set; }

    public int? EmployeeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? CompletedDate { get; set; }
}
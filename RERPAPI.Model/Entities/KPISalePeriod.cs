namespace RERPAPI.Model.Entities;

public partial class KPISalePeriod
{
    public int ID { get; set; }

    public string PeriodCode { get; set; } = null!;

    public string? PeriodName { get; set; }

    public string PeriodType { get; set; } = null!;

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public int? ParentPeriodID { get; set; }

    public bool IsClosed { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
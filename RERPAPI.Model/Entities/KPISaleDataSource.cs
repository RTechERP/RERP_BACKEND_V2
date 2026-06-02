namespace RERPAPI.Model.Entities;

public partial class KPISaleDataSource
{
    public int ID { get; set; }

    public string SourceCode { get; set; } = null!;

    public string SourceName { get; set; } = null!;

    public int AllowedTableID { get; set; }

    public string DateColumn { get; set; } = null!;

    public string? EmployeeColumn { get; set; }

    public string? ValueColumn { get; set; }

    public bool IsActive { get; set; }
}
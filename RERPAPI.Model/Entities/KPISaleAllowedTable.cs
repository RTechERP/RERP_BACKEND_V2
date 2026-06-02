namespace RERPAPI.Model.Entities;

public partial class KPISaleAllowedTable
{
    public int ID { get; set; }

    public string TableName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string SchemaName { get; set; } = null!;

    public bool IsActive { get; set; }
}
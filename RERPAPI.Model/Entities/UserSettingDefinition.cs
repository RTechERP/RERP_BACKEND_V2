namespace RERPAPI.Model.Entities;

public partial class UserSettingDefinition
{
    public int ID { get; set; }

    public string SettingGroup { get; set; } = null!;

    public string SettingKey { get; set; } = null!;

    public string SettingName { get; set; } = null!;

    public string? Description { get; set; }

    public string ValueType { get; set; } = null!;

    public string? DefaultValue { get; set; }

    public bool IsRequired { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
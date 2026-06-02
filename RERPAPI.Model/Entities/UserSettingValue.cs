namespace RERPAPI.Model.Entities;

public partial class UserSettingValue
{
    public int ID { get; set; }

    public int UserID { get; set; }

    public int SettingDefinitionID { get; set; }

    public string? SettingValue { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
namespace RERPAPI.Model.Entities;

public partial class KPISaleTemplate
{
    public int ID { get; set; }

    public string TemplateCode { get; set; } = null!;

    public string TemplateName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
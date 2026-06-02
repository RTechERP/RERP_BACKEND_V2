namespace RERPAPI.Model.Entities;

public partial class KPISaleSystemParameter
{
    public int ID { get; set; }

    public string ParamCode { get; set; } = null!;

    public string ParamName { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public bool IsActive { get; set; }
}
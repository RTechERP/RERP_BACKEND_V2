namespace RERPAPI.Model.Entities;

public partial class RequestInvoiceLog
{
    public int ID { get; set; }

    /// <summary>
    /// ID yêu cầu xuất hóa đơn
    /// </summary>
    public int? RequestInvoiceID { get; set; }

    /// <summary>
    /// Loại log
    /// </summary>
    public string? TypeLog { get; set; }

    /// <summary>
    /// Nội dung log
    /// </summary>
    public string? ContentLog { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
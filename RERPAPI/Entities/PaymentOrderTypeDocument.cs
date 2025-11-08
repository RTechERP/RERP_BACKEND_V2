using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PaymentOrderTypeDocument
{
    public int ID { get; set; }

    public int? PaymentOrderID { get; set; }

    /// <summary>
    /// 1: PO, 2: Hóa đơn
    /// </summary>
    public int? TypeDocumentID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

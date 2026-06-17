using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PaymentOrderTypeOrder
{
    public int ID { get; set; }

    public int? PaymentOrderID { get; set; }

    /// <summary>
    /// 1: Đề nghị tạm ứng, 2: Đề nghị thanh toán/quyết toán
    /// </summary>
    public int? TypeOrderID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

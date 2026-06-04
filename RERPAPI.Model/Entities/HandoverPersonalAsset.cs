using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HandoverPersonalAsset
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Name { get; set; }

    public string Code { get; set; } = null!;

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    /// <summary>
    /// Trạng thái sử dụng: 1 = Đang sử dụng, 2 = Chưa sử dụng
    /// </summary>
    public int? Status { get; set; }

    public int? ReceiverID { get; set; }

    public bool? IsSign { get; set; }

    public bool? IsHandover { get; set; }

    public int? HandoverID { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}

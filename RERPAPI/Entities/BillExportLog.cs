using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillExportLog
{
    public int ID { get; set; }

    public int? BillExportID { get; set; }

    /// <summary>
    /// 1: Nhận chứng từ hoặc đã nhận bill hoặc đã duyệt; 0: Chưa nhận hoặc là huỷ duyệt...
    /// </summary>
    public bool? StatusBill { get; set; }

    public DateTime? DateStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

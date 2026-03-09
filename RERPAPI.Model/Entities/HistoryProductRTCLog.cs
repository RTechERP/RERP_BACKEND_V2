using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryProductRTCLog
{
    /// <summary>
    /// ID log lịch sử mượn
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID lịch sử mượn
    /// </summary>
    public int? HistoryProductRTCID { get; set; }

    /// <summary>
    /// Ngày về dự kiến
    /// </summary>
    public DateTime? DateReturnExpected { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

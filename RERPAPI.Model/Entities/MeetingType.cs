using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MeetingType
{
    public int ID { get; set; }

    /// <summary>
    /// 1: Nội bộ; 2: Khách hàng
    /// </summary>
    public int? GroupID { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    public string? TypeContent { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDelete { get; set; }
}

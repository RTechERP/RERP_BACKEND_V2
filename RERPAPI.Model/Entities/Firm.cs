using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Firm
{
    public int ID { get; set; }

    public string? FirmCode { get; set; }

    public string? FirmName { get; set; }

    /// <summary>
    /// 1: Hãng kho Sale; 2: Hãng kho Demo
    /// </summary>
    public int? FirmType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDelete { get; set; }
}

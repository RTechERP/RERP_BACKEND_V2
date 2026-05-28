using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillImportQCDetailFile
{
    public int ID { get; set; }

    public int? BillImportQCDetailID { get; set; }

    public string? FileName { get; set; }

    public string? OriginPath { get; set; }

    public string? ServerPath { get; set; }

    /// <summary>
    /// 1: Pur checksheet,2: Tech report
    /// </summary>
    public int? FileType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

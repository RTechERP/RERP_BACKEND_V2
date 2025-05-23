using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DocumentSale
{
    public int ID { get; set; }

    /// <summary>
    /// ID của phiếu nhập hoặc xuất
    /// </summary>
    public int? BillID { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public string? FileNameOrigin { get; set; }

    /// <summary>
    /// 1: phiếu nhập; 2:Phiếu xuất
    /// </summary>
    public int? BillType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

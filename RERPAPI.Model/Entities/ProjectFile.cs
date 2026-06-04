using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectFile
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public string? FileName { get; set; }

    /// <summary>
    /// Loại file theo đuôi mở rộng
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// Kích thước file tính bằng bytes
    /// </summary>
    public decimal? Size { get; set; }

    public string? OriginPath { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// loại file theo thư mục (1: Video;2: Image)
    /// </summary>
    public int? FileTypeFolder { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng khai báo chức vụ được phép thực hiện hoặc phụ trách một bước (Step) trong quy trình Gate của dự án
/// </summary>
public partial class ProjectGateStepPosition
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID Step thuộc bảng ProjectGateStep
    /// </summary>
    public int ProjectGateStepID { get; set; }

    /// <summary>
    /// ID chức vụ thuộc bảng ChucVu
    /// </summary>
    public int ChucVuID { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }
}

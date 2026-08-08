using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Chi tiết danh sách checklist của từng bước Gate trong dự án
/// </summary>
public partial class ProjectGateStepCheckListDetail
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Quy định hoặc yêu cầu đối với tệp đính kèm
    /// </summary>
    public string? FileRule { get; set; }

    /// <summary>
    /// Định dạng tệp được phép (PDF, DOCX, XLSX, JPG...)
    /// </summary>
    public string? FileFormat { get; set; }

    /// <summary>
    /// Số lượng tệp yêu cầu
    /// </summary>
    public int FileQuantity { get; set; }

    /// <summary>
    /// Trạng thái checklist có check validate hay không (0: Chưa hoàn thành, 1: Đã hoàn thành)
    /// </summary>
    public bool IsCheck { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
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

    public bool? IsDeleted { get; set; }

    public int? ProjectGateStepID { get; set; }

    public bool IsFile { get; set; }

    public int? STT { get; set; }

    public string? FileName { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu file đính kèm của hợp đồng kinh tế
/// </summary>
public partial class EconomicContractFile
{
    /// <summary>
    /// ID bản ghi tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// ID hợp đồng kinh tế (EconomicContract)
    /// </summary>
    public int? EconomicContractID { get; set; }

    /// <summary>
    /// Tên file
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Đường dẫn gốc của file
    /// </summary>
    public string? OriginPath { get; set; }

    /// <summary>
    /// Đường dẫn file trên server
    /// </summary>
    public string? ServerPath { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 1-Đã xóa, 0-Chưa xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}

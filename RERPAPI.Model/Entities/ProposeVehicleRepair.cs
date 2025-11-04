using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Danh mục xe đi sửa chữa
/// </summary>
public partial class ProposeVehicleRepair
{
    /// <summary>
    /// ID của bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// ID bảng VehicleManagement
    /// </summary>
    public int? VehicleManagementID { get; set; }

    /// <summary>
    /// ID bảng VehicleRepairType
    /// </summary>
    public int? VehicleRepairTypeID { get; set; }

    /// <summary>
    /// Thời gian bắt đầu sửa chữa
    /// </summary>
    public DateTime? TimeStartRepair { get; set; }

    /// <summary>
    /// Thời gian kết thúc sửa chữa
    /// </summary>
    public DateTime? TimeEndRepair { get; set; }

    /// <summary>
    /// Lý do sửa chữa
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Nội dung đề xuất
    /// </summary>
    public string? ProposeContent { get; set; }

    /// <summary>
    /// ID nhân viên thực hiện đem xe đi sửa
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    public int? EmployeeProposeID { get; set; }

    public DateTime? DatePropose { get; set; }
}

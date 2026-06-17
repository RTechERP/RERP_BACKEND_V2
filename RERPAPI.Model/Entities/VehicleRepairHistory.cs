using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Danh sách sửa chữa của xe sau khi phê duyệt đề xuất
/// </summary>
public partial class VehicleRepairHistory
{
    /// <summary>
    /// ID của bản ghi (tự tăng)
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
    /// ID chi tiết đề xuất đã chọn
    /// </summary>
    public int? ProposeVehicleRepairDetailID { get; set; }

    /// <summary>
    /// ID người duyệt
    /// </summary>
    public int? ApproveID { get; set; }

    /// <summary>
    /// Ngày ghi lịch sử
    /// </summary>
    public DateTime? DateReport { get; set; }

    /// <summary>
    /// Kiểu sửa chữa (ID)
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
    /// Nội dung đề xuất lúc duyệt
    /// </summary>
    public string? ProposeContent { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Tên gara sửa chữa
    /// </summary>
    public string? GaraName { get; set; }

    /// <summary>
    /// SĐT gara
    /// </summary>
    public string? SDTGara { get; set; }

    /// <summary>
    /// Địa chỉ gara
    /// </summary>
    public string? AddressGara { get; set; }

    /// <summary>
    /// Đơn vị tính
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Đơn giá
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Tổng tiền
    /// </summary>
    public decimal? TotalPrice { get; set; }

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

    public DateTime? DateApprove { get; set; }

    public int? WarrantyPeriod { get; set; }

    public int? KmPreviousPeriod { get; set; }

    public int? KmCurrentPeriod { get; set; }

    public DateTime? TimePrevious { get; set; }
}

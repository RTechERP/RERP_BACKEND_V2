using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Danh mục gara đề xuất, khi chọn 1 trong số gara sẽ vớt sang theo dõi
/// </summary>
public partial class ProposeVehicleRepairDetail
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
    /// ID bảng VehicleRepairPropose (Bảng master)
    /// </summary>
    public int? VehicleRepairProposeID { get; set; }

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

    public int? IsApprove { get; set; }

    public int? ApproveID { get; set; }

    public DateTime? DateApprove { get; set; }

    public int? Warranty { get; set; }

    public int? WarrantyPeriod { get; set; }
}

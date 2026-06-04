using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVProduct
{
    public int ID { get; set; }

    /// <summary>
    /// Nhóm sản phẩm lấy từ bảng AGVProductGroup
    /// </summary>
    public int? AGVProductGroupID { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    /// <summary>
    /// Hãng lấy từ bảng Firm
    /// </summary>
    public int? FirmID { get; set; }

    /// <summary>
    /// Đơn vị tính lấy từ bảng UnitCount
    /// </summary>
    public int? UnitCountID { get; set; }

    /// <summary>
    /// Vị trí sản phẩm lấy từ AGVProductLocation
    /// </summary>
    public int? AGVProductLocationID { get; set; }

    public string? Note { get; set; }

    public string? Resolution { get; set; }

    public string? MonoColor { get; set; }

    public string? SensorSize { get; set; }

    public string? DataInterface { get; set; }

    public string? LensMount { get; set; }

    public string? ShutterMode { get; set; }

    public string? PixelSize { get; set; }

    public string? SensorSizeMax { get; set; }

    public string? MOD { get; set; }

    public string? FNo { get; set; }

    public string? WD { get; set; }

    public string? LampType { get; set; }

    public string? LampColor { get; set; }

    public string? LampPower { get; set; }

    public string? LampWattage { get; set; }

    public string? Magnification { get; set; }

    public string? FocalLength { get; set; }

    public string? InputValue { get; set; }

    public string? OutputValue { get; set; }

    public string? CurrentIntensityMax { get; set; }

    public string? Size { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Hãng
    /// </summary>
    public string? Maker { get; set; }

    /// <summary>
    /// Số lượng tổng
    /// </summary>
    public decimal? Number { get; set; }

    public string? AddressBox { get; set; }

    /// <summary>
    /// Trạng thái sản phẩm (1: hiện có, 0: không có)
    /// </summary>
    public bool? StatusProduct { get; set; }

    public DateTime? CreateDate { get; set; }

    public decimal? NumberInStore { get; set; }

    public string? Serial { get; set; }

    public string? SerialNumber { get; set; }

    public string? PartNumber { get; set; }

    public string? LocationImg { get; set; }

    public string? AGVProductCode { get; set; }

    public bool? BorrowCustomer { get; set; }

    public int? SLKiemKe { get; set; }

    public int? WarehouseID { get; set; }

    /// <summary>
    /// 1: Đang giặt
    /// </summary>
    public int? Status { get; set; }

    public string? CodeHCM { get; set; }
}

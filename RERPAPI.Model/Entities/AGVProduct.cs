using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVProduct
{
    public int ID { get; set; }

    /// <summary>
    /// Nhóm sản phẩm lấy từ bảng ProductGroupRTC
    /// </summary>
    public int? ProductGroupRTCID { get; set; }

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
    /// Vị trí sản phẩm lấy từ ProductLocation
    /// </summary>
    public int? ProductLocationID { get; set; }

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
}

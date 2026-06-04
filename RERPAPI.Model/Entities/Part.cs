using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Part
{
    public int ID { get; set; }

    /// <summary>
    /// Thuộc nhóm vật tư nào
    /// </summary>
    public int? PartGroupID { get; set; }

    /// <summary>
    /// Thuộc hãng nào
    /// </summary>
    public int? ManufacturerID { get; set; }

    /// <summary>
    /// Mã vật tư thiết bị theo hãng cung cấp
    /// </summary>
    public string PartCode { get; set; } = null!;

    /// <summary>
    /// Mã vật tư thiết bị theo RTC
    /// </summary>
    public string? PartCodeRTC { get; set; }

    /// <summary>
    /// Tên vật tư thiết bị
    /// </summary>
    public string PartName { get; set; } = null!;

    public string? PartNameRTC { get; set; }

    /// <summary>
    /// Trạng thái vật tư: 1: đang sử dụng,0: ngừng sử dụng
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Giá gần nhất
    /// </summary>
    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Có được sử dụng hay không
    /// </summary>
    public bool? IsDeleted { get; set; }
}

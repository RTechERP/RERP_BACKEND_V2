using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductRTCQRCode
{
    public int ID { get; set; }

    /// <summary>
    /// 1:Trong Kho,2:Đang mượn,3:Đã Xuất Kho
    /// </summary>
    public int? Status { get; set; }

    public int? ProductRTCID { get; set; }

    public string? ProductQRCode { get; set; }

    public string? Serial { get; set; }

    public string? SerialNumber { get; set; }

    public string? PartNumber { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? WarehouseID { get; set; }

    public int? ModulaLocationDetailID { get; set; }
}

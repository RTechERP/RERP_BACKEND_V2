using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class InventoryProject
{
    /// <summary>
    /// ID tồn kho giữ dự án
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// ID sản phẩm kho sale
    /// </summary>
    public int? ProductSaleID { get; set; }

    /// <summary>
    /// ID Người giữ
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    /// <summary>
    /// Số lượng giữ
    /// </summary>
    public decimal? Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// ID chi tiết Pokh
    /// </summary>
    public int? POKHDetailID { get; set; }

    /// <summary>
    /// ID khách hàng
    /// </summary>
    public int? CustomerID { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Số lượng ban đầu
    /// </summary>
    public decimal? QuantityOrigin { get; set; }

    /// <summary>
    /// ID cha
    /// </summary>
    public int? ParentID { get; set; }
}

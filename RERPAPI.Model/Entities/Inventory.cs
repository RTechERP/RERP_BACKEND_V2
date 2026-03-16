using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Inventory
{
    /// <summary>
    /// ID tồn kho
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID sản phẩm kho sale
    /// </summary>
    public int? ProductSaleID { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    /// <summary>
    /// Tồn đầu kỳ
    /// </summary>
    public decimal? TotalQuantityFirst { get; set; }

    /// <summary>
    /// Số lượng nhập
    /// </summary>
    public decimal? Import { get; set; }

    /// <summary>
    /// Số lượng xuất
    /// </summary>
    public decimal? Export { get; set; }

    /// <summary>
    /// Tồn cuối kỳ
    /// </summary>
    public decimal? TotalQuantityLast { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Hàng stock
    /// </summary>
    public bool? IsStock { get; set; }

    /// <summary>
    /// Số lượng tối thiểu
    /// </summary>
    public decimal? MinQuantity { get; set; }

    /// <summary>
    /// Nhân viên thêm stock
    /// </summary>
    public int? EmployeeStock { get; set; }
}

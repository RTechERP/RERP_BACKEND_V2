using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng liên kết SupplierSale và EmployeePurchase
/// </summary>
public partial class SupplierSaleLink
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID bảng SupplierSale
    /// </summary>
    public int SupplierSaleID { get; set; }

    /// <summary>
    /// ID bảng EmployeePurchase
    /// </summary>
    public int EmployeePurchaseID { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Mặt hàng
    /// </summary>
    public string? MatHang { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Nhà cung cấp
/// </summary>
public partial class Supplier
{
    public int ID { get; set; }

    public string SupplierName { get; set; } = null!;

    public string SupplierCode { get; set; } = null!;

    public string? SupplierShortName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public string? MST { get; set; }

    public string? BankName { get; set; }

    public string? BankAcount { get; set; }

    public string? BankAcountName { get; set; }

    public string? Office { get; set; }

    public string? Address { get; set; }

    /// <summary>
    /// Sản phẩm chính
    /// </summary>
    public string? MainProduct { get; set; }

    /// <summary>
    /// Hạn mức công nợ
    /// </summary>
    public decimal? DebtLimit { get; set; }

    public bool? IsDeleted { get; set; }

    public string? SkypeID { get; set; }

    /// <summary>
    /// Hãng cung cấp
    /// </summary>
    public string? Manufactures { get; set; }

    /// <summary>
    /// Ưu điểm
    /// </summary>
    public string? Advantages { get; set; }

    /// <summary>
    /// Nhược điểm
    /// </summary>
    public string? Defect { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

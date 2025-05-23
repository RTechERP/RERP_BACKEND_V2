using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Khách hàng
/// </summary>
public partial class Customer
{
    public int ID { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerCode { get; set; } = null!;

    public string? CustomerShortName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// 0: other, 1: nhà máy, 2: thương mại
    /// </summary>
    public int? CustomerType { get; set; }

    public int? StatusDisable { get; set; }

    public string? Website { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactNote { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? NoteDelivery { get; set; }

    public string? NoteVoucher { get; set; }

    public string? CheckVoucher { get; set; }

    public string? HardCopyVoucher { get; set; }

    public DateTime? ClosingDateDebt { get; set; }

    public string? Debt { get; set; }

    public string? AdressStock { get; set; }

    public string? TaxCode { get; set; }

    public string? CustomerDetails { get; set; }

    public int? CustomerSpecializationID { get; set; }

    public string? ProductDetails { get; set; }

    public bool? BigAccount { get; set; }

    public string? Province { get; set; }

    public virtual ICollection<RegisterOT> RegisterOTs { get; set; } = new List<RegisterOT>();
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class QuotationKH
{
    public int ID { get; set; }

    public string? QuotationCode { get; set; }

    public string? Version { get; set; }

    public int? ProjectID { get; set; }

    /// <summary>
    /// ID tài khoản đăng nhập
    /// </summary>
    public int? UserID { get; set; }

    public string? POCode { get; set; }

    public int? CustomerID { get; set; }

    public string? Explanation { get; set; }

    public bool? IsApproved { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? QuotationDate { get; set; }

    public int? Status { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? CreatedBy { get; set; }

    public int? ContactID { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    /// <summary>
    /// Người phụ trách
    /// </summary>
    public int? UserName { get; set; }

    public DateTime? CreateDate { get; set; }

    public decimal? Commission { get; set; }

    public decimal? ComMoney { get; set; }

    public string? AttachFile { get; set; }

    public string? Company { get; set; }

    public bool? IsMerge { get; set; }

    public bool? IsDeleted { get; set; }
}

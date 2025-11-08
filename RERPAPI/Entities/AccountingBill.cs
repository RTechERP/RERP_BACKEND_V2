using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class AccountingBill
{
    public int ID { get; set; }

    public string? BillNumber { get; set; }

    public DateTime? BillDate { get; set; }

    public string? SupplierSale { get; set; }

    public int? SupplierSaleID { get; set; }

    public decimal? TotalMoney { get; set; }

    public int? CurrencyID { get; set; }

    public int? TaxCompanyID { get; set; }

    public int? EmployeeID { get; set; }

    /// <summary>
    /// Xác nhận của Pur; 1: đã xác nhân, 0: chưa xác nhận
    /// </summary>
    public int? EmployeeStatus { get; set; }

    /// <summary>
    /// Bàn giao cho BP thuế; 1: đã xác nhân, 0: chưa xác nhận
    /// </summary>
    public int? DeliverTaxStatus { get; set; }

    public DateTime? DeliverTaxDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}

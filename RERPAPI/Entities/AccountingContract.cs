using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class AccountingContract
{
    public int ID { get; set; }

    public DateTime? DateInput { get; set; }

    /// <summary>
    /// 1:RTC; 2:MVI; 3: APR; 4:YONKO
    /// </summary>
    public int? Company { get; set; }

    /// <summary>
    /// Loại HĐ chính (1:Hợp đồng mua vào; 2:Hợp đồng bán ra)
    /// </summary>
    public int? ContractGroup { get; set; }

    public int? AccountingContractTypeID { get; set; }

    public int? CustomerID { get; set; }

    public int? SupplierSaleID { get; set; }

    public string? ContractNumber { get; set; }

    public DateTime? DateContract { get; set; }

    public string? ContractContent { get; set; }

    public decimal? ContractValue { get; set; }

    public string? ContentPayment { get; set; }

    public string? Unit { get; set; }

    public DateTime? DateExpired { get; set; }

    /// <summary>
    /// Ngày duyệt trên nhóm
    /// </summary>
    public DateTime? DateIsApprovedGroup { get; set; }

    public int? EmployeeID { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// 1: đã nhận hđ gốc; 0: chưa nhận
    /// </summary>
    public bool? IsReceivedContract { get; set; }

    /// <summary>
    /// Ngày nhận hồ sơ gốc
    /// </summary>
    public DateTime? DateReceived { get; set; }

    public int? QuantityDocument { get; set; }

    public bool? IsApproved { get; set; }

    public int? ParentID { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

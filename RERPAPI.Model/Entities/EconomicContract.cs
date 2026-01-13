using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin hợp đồng kinh tế
/// </summary>
public partial class EconomicContract
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? EconomicContractTypeID { get; set; }

    public int? EconomicContractTermID { get; set; }

    public string? ContractNumber { get; set; }

    public string? ContractContent { get; set; }

    public string? NameNcc { get; set; }

    public string? MSTNcc { get; set; }

    public string? AddressNcc { get; set; }

    public string? SDTNcc { get; set; }

    public string? EmailNcc { get; set; }

    public decimal? SignedAmount { get; set; }

    public string? MoneyType { get; set; }

    public string? TimeUnit { get; set; }

    public string? Adjustment { get; set; }

    public string? Note { get; set; }

    public DateTime? SignDate { get; set; }

    public DateTime? EffectDateFrom { get; set; }

    public DateTime? EffectDateTo { get; set; }

    public int? StatusContract { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 1 = NCC, 2 = KH
    /// </summary>
    public int? TypeNCC { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AccountingContractType
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    /// <summary>
    /// 0: Không có giá trị; 1:Có giá trị
    /// </summary>
    public bool? IsContractValue { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Danh mục loại hợp đồng kinh tế
/// </summary>
public partial class EconomicContractType
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}

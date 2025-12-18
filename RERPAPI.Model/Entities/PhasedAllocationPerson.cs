using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PhasedAllocationPerson
{
    public int ID { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// 1: Quà; 2: Tài sản cá nhân
    /// </summary>
    public int? TypeAllocation { get; set; }

    public string? Code { get; set; }

    public string? ContentAllocation { get; set; }

    public int? YearValue { get; set; }

    public int? MontValue { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 0: Chưa hoàn thành; 1: Đã hoàn thành
    /// </summary>
    public int? StatusAllocation { get; set; }
}

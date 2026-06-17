using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FilmManagementDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? FilmManagementID { get; set; }

    /// <summary>
    /// Lấy từ UnitCount
    /// </summary>
    public int? UnitID { get; set; }

    public decimal? PerformanceAVG { get; set; }

    public string? WorkContent { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

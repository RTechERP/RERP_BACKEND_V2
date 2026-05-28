using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIEvaluationFactor
{
    public int ID { get; set; }

    /// <summary>
    /// Năm đánh giá
    /// </summary>
    public int? KPIExamID { get; set; }

    /// <summary>
    /// 1: ĐÁNH GIÁ KỸ NĂNG
    /// , 2: CHUYÊN MÔN
    /// </summary>
    public int? EvaluationType { get; set; }

    /// <summary>
    /// 1: Kỹ năng; 2: PLC, Robot; 3: VISION; 4: SOFTWARE
    /// </summary>
    public int? SpecializationType { get; set; }

    public string? STT { get; set; }

    public string? EvaluationContent { get; set; }

    /// <summary>
    /// Phương tiện xác minh tiêu chí
    /// </summary>
    public string? VerificationToolsContent { get; set; }

    public decimal? StandardPoint { get; set; }

    public int? Coefficient { get; set; }

    public int? ParentID { get; set; }

    public string? Unit { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

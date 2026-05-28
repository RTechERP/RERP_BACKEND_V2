using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPISumarize
{
    public int ID { get; set; }

    public int? YearEvalution { get; set; }

    public int? QuarterEvalution { get; set; }

    public int? EmployeeID { get; set; }

    /// <summary>
    /// Thời gian, giờ giấc
    /// </summary>
    public int? TimeHours { get; set; }

    /// <summary>
    /// 5S quy trình, quy định
    /// </summary>
    public int? FiveSRegulatedProcedures { get; set; }

    /// <summary>
    /// Chuẩn bị hàng và báo cáo công việc
    /// </summary>
    public int? PrepareGoodsReport { get; set; }

    /// <summary>
    /// Tinh thần làm việc
    /// </summary>
    public int? AttitudeTowardsCustomers { get; set; }

    /// <summary>
    /// Làm mất thiết bị
    /// </summary>
    public int? LossEquipment { get; set; }

    /// <summary>
    /// Điểm kỹ năng
    /// </summary>
    public decimal? SkillPoints { get; set; }

    /// <summary>
    /// Điểm chuyên môn PLC
    /// </summary>
    public decimal? PLCExpertisePoints { get; set; }

    /// <summary>
    /// Điểm chuyên môn Vision
    /// </summary>
    public decimal? VisionExpertisePoints { get; set; }

    /// <summary>
    /// Điểm chuyên môn Software
    /// </summary>
    public decimal? SoftwareExpertisePoints { get; set; }
}

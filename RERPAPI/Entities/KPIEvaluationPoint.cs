using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIEvaluationPoint
{
    public int ID { get; set; }

    public int? KPIEvaluationFactorsID { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? EmployeePoint { get; set; }

    public decimal? TBPPoint { get; set; }

    public int? TBPID { get; set; }

    public decimal? BGDPoint { get; set; }

    public int? BGDID { get; set; }

    public decimal? EmployeeEvaluation { get; set; }

    public decimal? EmployeeCoefficient { get; set; }

    public decimal? TBPEvaluation { get; set; }

    public decimal? TBPCoefficient { get; set; }

    public decimal? BGDEvaluation { get; set; }

    public decimal? BGDCoefficient { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// 1: Hoàn thành; 0: Chưa hoàn thành
    /// </summary>
    public int? Status { get; set; }

    public bool? IsAdminConfirm { get; set; }

    public decimal? TBPPointInput { get; set; }

    public decimal? BGDPointInput { get; set; }

    public DateTime? DateEmployeeConfirm { get; set; }

    public DateTime? DateTBPConfirm { get; set; }

    public DateTime? DateBGDConfirm { get; set; }
}

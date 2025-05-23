using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryKPISale
{
    public int ID { get; set; }

    public int? MainIndexID { get; set; }

    public decimal? Goal0 { get; set; }

    public decimal? Goal1 { get; set; }

    public decimal? Goal2 { get; set; }

    public decimal? Result0 { get; set; }

    public decimal? Result1 { get; set; }

    public decimal? Result2 { get; set; }

    public decimal? ACCP0 { get; set; }

    public decimal? ACCP1 { get; set; }

    public decimal? ACCP2 { get; set; }

    public decimal? Goal { get; set; }

    public decimal? Result { get; set; }

    public decimal? ACCP { get; set; }

    public decimal? PercentIndex { get; set; }

    public int? Quy { get; set; }

    public int? Year { get; set; }

    public int? UserID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

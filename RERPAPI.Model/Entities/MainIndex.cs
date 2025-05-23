using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MainIndex
{
    public int ID { get; set; }

    public string? MainIndex1 { get; set; }

    public int? MainGroup { get; set; }

    public decimal? Goal0 { get; set; }

    public decimal? Result0 { get; set; }

    public decimal? ACCP0 { get; set; }

    public decimal? Goal1 { get; set; }

    public decimal? Result1 { get; set; }

    public decimal? ACCP1 { get; set; }

    public decimal? Goal2 { get; set; }

    public decimal? Result2 { get; set; }

    public decimal? ACCP2 { get; set; }

    public decimal? ACCP { get; set; }

    public decimal? Goal { get; set; }

    public decimal? Result { get; set; }

    public int? ConvertID { get; set; }
}

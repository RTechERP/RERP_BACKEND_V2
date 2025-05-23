﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSSourceAsset
{
    public int ID { get; set; }

    public string? SourceCode { get; set; }

    public string? SourceName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

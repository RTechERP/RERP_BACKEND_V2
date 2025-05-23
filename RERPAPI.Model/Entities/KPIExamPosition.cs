﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIExamPosition
{
    public int ID { get; set; }

    public int? KPIExamID { get; set; }

    public int? KPIPositionID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

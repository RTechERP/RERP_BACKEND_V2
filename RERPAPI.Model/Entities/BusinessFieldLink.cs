﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BusinessFieldLink
{
    public int ID { get; set; }

    public int? BusinessFieldID { get; set; }

    public int? CustomerID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

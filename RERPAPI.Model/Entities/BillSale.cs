﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillSale
{
    public int ID { get; set; }

    public string? BillCode { get; set; }

    public string? ProductName { get; set; }
}

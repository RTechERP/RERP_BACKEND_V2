﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FirmBase
{
    public int ID { get; set; }

    public string? FirmCode { get; set; }

    public string? FirmName { get; set; }
}

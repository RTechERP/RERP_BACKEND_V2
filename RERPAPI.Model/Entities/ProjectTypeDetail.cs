﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTypeDetail
{
    public int ID { get; set; }

    public int? ProjectTypeID { get; set; }

    public string? ProjectTypeNameChild { get; set; }
}

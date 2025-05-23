﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PONCCDetailRequestBuy
{
    public int ID { get; set; }

    public int? PONCCDetailID { get; set; }

    public int? ProjectPartlistPurchaseRequestID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

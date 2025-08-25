﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSAssetRecovery
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public DateTime? DateRecovery { get; set; }

    public int? EmployeeReturnID { get; set; }

    public int? EmployeeRecoveryID { get; set; }

    /// <summary>
    /// 0: chưa duyệt; 1: Đã duyệt
    /// </summary>
    public int? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsApproveAccountant { get; set; }

    public bool? IsApprovedPersonalProperty { get; set; }

    public DateTime? DateApproveAccountant { get; set; }

    public DateTime? DateApprovedPersonalProperty { get; set; }

    public DateTime? DateApprovedHR { get; set; }
}

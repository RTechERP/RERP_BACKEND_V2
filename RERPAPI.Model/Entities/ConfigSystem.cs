﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ConfigSystem
{
    public int ID { get; set; }

    public string KeyName { get; set; } = null!;

    public string? KeyValue { get; set; }

    public string? KeyValue1 { get; set; }

    public string? KeyValue2 { get; set; }

    public string? KeyValue3 { get; set; }

    public string? KeyValue4 { get; set; }

    public string? KeyValue5 { get; set; }

    public string? KeyValue6 { get; set; }

    public string? KeyValue7 { get; set; }

    public string? KeyValue8 { get; set; }

    public string? KeyValue9 { get; set; }

    public string? KeyValue10 { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// 1: Tiền tệ,2: Cấu hình mail
    /// </summary>
    public int? ConfigType { get; set; }

    public int? UserID { get; set; }
}

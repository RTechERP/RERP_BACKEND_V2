﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ModulaLocation
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// Đơn vị chiều rộng theo mm
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Đơn vị chiều dài theo mm
    /// </summary>
    public int? Height { get; set; }

    public int? AxisX { get; set; }

    public int? AxisY { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

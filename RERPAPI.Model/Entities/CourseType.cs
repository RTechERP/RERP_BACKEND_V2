﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseType
{
    public int ID { get; set; }

    public string? CourseTypeCode { get; set; }

    public string? CourseTypeName { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? STT { get; set; }

    public bool? IsDeleted { get; set; }
}

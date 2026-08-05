using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseCatalogType
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? CourseCatalogTypeCode { get; set; }

    public string? CourseCatalogTypeName { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}

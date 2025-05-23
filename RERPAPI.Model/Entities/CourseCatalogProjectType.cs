using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseCatalogProjectType
{
    public int ID { get; set; }

    public int? CourseCatalogID { get; set; }

    public int? ProjectTypeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

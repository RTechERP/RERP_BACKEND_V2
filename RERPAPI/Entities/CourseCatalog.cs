using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class CourseCatalog
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? DepartmentID { get; set; }

    public bool? DeleteFlag { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// 1:Cơ bản; 2:Nâng cao
    /// </summary>
    public int? CatalogType { get; set; }
}

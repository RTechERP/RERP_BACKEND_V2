using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ESLTestTable
{
    public int ID { get; set; }

    public string TestTableName { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public int TableSide { get; set; }

    public int? NumberOfSides { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<ESLTestTableRegistration> ESLTestTableRegistrations { get; set; } = new List<ESLTestTableRegistration>();
}

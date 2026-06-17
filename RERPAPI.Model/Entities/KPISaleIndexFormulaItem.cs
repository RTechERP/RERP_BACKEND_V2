using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleIndexFormulaItem
{
    public int ID { get; set; }

    public int ParentKpiIndexID { get; set; }

    public int ChildKpiIndexID { get; set; }

    public string Operator { get; set; } = null!;

    public int SortOrder { get; set; }
}

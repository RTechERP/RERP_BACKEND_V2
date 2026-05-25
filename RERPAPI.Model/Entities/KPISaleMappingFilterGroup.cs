using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleMappingFilterGroup
{
    public int ID { get; set; }

    public int MappingID { get; set; }

    public int? ParentGroupID { get; set; }

    public string LogicOperator { get; set; } = null!;

    public int SortOrder { get; set; }
}

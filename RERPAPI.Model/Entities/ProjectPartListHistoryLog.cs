using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartListHistoryLog
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? ProjectPartListVersionID { get; set; }

    public int? ProjectPartListID { get; set; }

    public string? ActionType { get; set; }

    public string? ContentLog { get; set; }

    public string? CreatedBy { get; set; }

    public int? CreatedByEmployeeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ActionHistory
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public string? UserName { get; set; }

    public string? Action { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

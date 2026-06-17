using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectUser
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? ProjectID { get; set; }

    public int? UserID { get; set; }

    public string? Mission { get; set; }
}

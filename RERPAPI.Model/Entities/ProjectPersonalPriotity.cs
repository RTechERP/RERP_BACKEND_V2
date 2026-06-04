using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPersonalPriotity
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public int? ProjectID { get; set; }

    public int? Priotity { get; set; }
}

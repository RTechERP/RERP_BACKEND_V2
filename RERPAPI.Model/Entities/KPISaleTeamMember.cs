using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleTeamMember
{
    public int ID { get; set; }

    public int TeamID { get; set; }

    public int EmployeeID { get; set; }

    public DateTime CreatedDate { get; set; }
}

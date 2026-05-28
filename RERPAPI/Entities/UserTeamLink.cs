using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class UserTeamLink
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public int? UserTeamID { get; set; }
}

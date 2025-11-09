using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class UserGroupLink
{
    public int ID { get; set; }

    public int? UserGroupID { get; set; }

    public int? UserID { get; set; }
}

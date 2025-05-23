using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class UserGroupRightDistribution
{
    public int ID { get; set; }

    public int FormAndFunctionID { get; set; }

    public int UserGroupID { get; set; }
}

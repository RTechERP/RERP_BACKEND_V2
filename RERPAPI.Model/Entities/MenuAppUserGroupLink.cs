using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MenuAppUserGroupLink
{
    public int ID { get; set; }

    public int? MenuAppID { get; set; }

    public int? UserGroupID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}

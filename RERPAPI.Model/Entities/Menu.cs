using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Menu
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? MenuName { get; set; }

    public string? Link { get; set; }

    public string? ImageName { get; set; }

    public int? ParentID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

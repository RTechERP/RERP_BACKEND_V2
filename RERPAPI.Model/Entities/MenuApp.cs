using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MenuApp
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? Title { get; set; }

    public string? Router { get; set; }

    public string? QueryParam { get; set; }

    public string? Icon { get; set; }

    public int? ParentID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}

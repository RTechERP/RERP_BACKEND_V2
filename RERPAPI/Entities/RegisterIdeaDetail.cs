using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RegisterIdeaDetail
{
    public int ID { get; set; }

    public int? RegisterIdeaID { get; set; }

    public int? STT { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }
}

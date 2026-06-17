using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class RequestImport
{
    public int ID { get; set; }

    public bool? IsApproved { get; set; }

    public string? ImportCode { get; set; }

    public string? Requester { get; set; }

    public string? Importer { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ImportDate { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

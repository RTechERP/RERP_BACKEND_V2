using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class JobRequirementCheckPrice
{
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Customer { get; set; }

    public string? ProductCode { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? HRSuggestion { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}

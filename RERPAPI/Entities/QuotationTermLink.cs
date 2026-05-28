using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class QuotationTermLink
{
    public int ID { get; set; }

    public int? QuotationID { get; set; }

    public int? TermConditionID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

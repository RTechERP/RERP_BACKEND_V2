using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TrackingMarksTaxCompany
{
    public int ID { get; set; }

    public int? TrackingMartkID { get; set; }

    public int? TaxCompanyID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

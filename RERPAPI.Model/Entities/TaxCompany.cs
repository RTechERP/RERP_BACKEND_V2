using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TaxCompany
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? TaxCode { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Director { get; set; }

    public string? Position { get; set; }

    public string? FullName { get; set; }
}

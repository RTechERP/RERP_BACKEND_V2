using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeError
{
    public int ID { get; set; }

    public bool? IsApproved { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? Money { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DateError { get; set; }

    public bool? IsDeleted { get; set; }
}

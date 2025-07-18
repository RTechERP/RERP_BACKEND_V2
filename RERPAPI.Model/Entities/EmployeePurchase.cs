﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeePurchase
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? TaxCompayID { get; set; }

    public string? Telephone { get; set; }

    public string? Email { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? FullName { get; set; }
}

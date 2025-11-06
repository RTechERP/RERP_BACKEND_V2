using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeTypeBussiness
{
    public int ID { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    public decimal? Cost { get; set; }
    public bool? IsDeleted { get; set; }

}

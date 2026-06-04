using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CustomerBaseContact
{
    public int ID { get; set; }

    public int? CustomerID { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? CustomerPosition { get; set; }

    public string? Note { get; set; }
}

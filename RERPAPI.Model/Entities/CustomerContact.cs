using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CustomerContact
{
    public int ID { get; set; }

    public int? CustomerID { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CustomerTeam { get; set; }

    public string? CustomerPart { get; set; }

    public string? CustomerPosition { get; set; }

    public bool? IsDeleted { get; set; }
}

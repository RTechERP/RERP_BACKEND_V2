using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CustomerBase
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public string? CustomerCode { get; set; }

    public string? CustomerName { get; set; }

    public string? Address { get; set; }

    public string? Province { get; set; }

    public string? KCN { get; set; }

    public int? CustomerType { get; set; }

    public string? ProductName { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}

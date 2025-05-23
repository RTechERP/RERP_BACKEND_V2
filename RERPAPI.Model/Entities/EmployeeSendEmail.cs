using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeSendEmail
{
    public int ID { get; set; }

    public string? Subject { get; set; }

    public string? EmailTo { get; set; }

    public string? EmailCC { get; set; }

    public string? Body { get; set; }

    public int? StatusSend { get; set; }

    public DateTime? DateSend { get; set; }

    public int? EmployeeID { get; set; }

    public int? Receiver { get; set; }

    public string? TableInfor { get; set; }
}

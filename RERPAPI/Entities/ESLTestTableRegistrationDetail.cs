using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ESLTestTableRegistrationDetail
{
    public int ID { get; set; }

    public int RegistrationID { get; set; }

    public int No { get; set; }

    public int Type { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTime? ActualReturnDate { get; set; }

    public int OwnerID { get; set; }

    public int ApproverID { get; set; }

    public int? Status { get; set; }

    public DateTime? ApproveDate { get; set; }

    public string? ApproveNote { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ESLTestTableRegistration Registration { get; set; } = null!;
}

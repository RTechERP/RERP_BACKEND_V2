using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FiveSRatingTicket
{
    public int ID { get; set; }

    public string? TicketCode { get; set; }

    public int? Rating5SID { get; set; }

    public int? EmployeeRating1ID { get; set; }

    public int? EmployeeRating2ID { get; set; }

    public DateTime? TicketDate { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

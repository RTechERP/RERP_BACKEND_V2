using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MeetingMinute
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? MeetingTypeID { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? Title { get; set; }

    public string? Place { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}

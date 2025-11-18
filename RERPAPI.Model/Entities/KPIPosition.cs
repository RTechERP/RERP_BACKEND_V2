using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIPosition
{
    public int ID { get; set; }

    public string? PositionCode { get; set; }

    public string? PositionName { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// 1: Kỹ thuật, Pro; 2: Admin; 3: Senior; 4: Phó phòng
    /// </summary>
    public int? TypePosition { get; set; }

    public int? KPISessionID { get; set; }

    public int? KPIPositionTypeID { get; set; }
}

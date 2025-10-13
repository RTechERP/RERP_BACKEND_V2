﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeBussiness
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public bool? IsApproved { get; set; }

    /// <summary>
    /// Trưởng phòng duyệt
    /// </summary>
    public int? ApprovedID { get; set; }

    public DateTime? DayBussiness { get; set; }

    /// <summary>
    /// Loại công tác: 1.Công tác ngày; 2.Công tác đêm; 3. Công tác gần (10km - 30km); 4.Công tác xa
    /// </summary>
    public int? TypeBusiness { get; set; }

    public string? Location { get; set; }

    public int? VehicleID { get; set; }

    public decimal? CostVehicle { get; set; }

    public decimal? CostBussiness { get; set; }

    public decimal? TotalMoney { get; set; }

    /// <summary>
    /// true: Không chấm công ở văn phòng
    /// </summary>
    public bool? NotChekIn { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? Overnight { get; set; }

    public decimal? CostOvernight { get; set; }

    public bool? WorkEarly { get; set; }

    public decimal? CostWorkEarly { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public int? ApprovedHR { get; set; }

    public bool? IsApprovedHR { get; set; }

    public string? ReasonDeciline { get; set; }

    /// <summary>
    /// 1:Phụ cấp ăn tối từ sau 20h; 2:Phụ cấp ăn tối theo loại công tác
    /// </summary>
    public int? OvernightType { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsProblem { get; set; }

    public string? Reason { get; set; }

    public int? ProvinceID { get; set; }
}

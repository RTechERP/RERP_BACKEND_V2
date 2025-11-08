using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeEducationLevel
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? STT { get; set; }

    public string? SchoolName { get; set; }

    /// <summary>
    /// 1:Đại học (ĐH); 2: Cao đẳng (CĐ); 3: Trung cấp (TC)
    /// </summary>
    public int? RankType { get; set; }

    /// <summary>
    /// Loại hình đào tạo (1. Chính quy;2. Liên thông)
    /// </summary>
    public int? TrainType { get; set; }

    public string? Major { get; set; }

    public int? YearGraduate { get; set; }

    /// <summary>
    /// Xếp loại (1:Giỏi; 2: Khá, 3: Trung bình)
    /// </summary>
    public int? Classification { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

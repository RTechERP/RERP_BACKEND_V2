using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeFamily
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public string? TenNguoiThan { get; set; }

    public bool? PhuThuoc { get; set; }

    public string? SoBHXH { get; set; }

    public DateTime? NgaySinh { get; set; }

    /// <summary>
    /// values(0,Nam) ,(1,Nữ),(2,Khác)
    /// </summary>
    public int? GioiTinh { get; set; }

    public string? QuocTich { get; set; }

    public string? DanToc { get; set; }

    public string? NoiKhaiSinh { get; set; }

    public string? QuanHe { get; set; }

    public string? CMND { get; set; }

    public string? GhiChu { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

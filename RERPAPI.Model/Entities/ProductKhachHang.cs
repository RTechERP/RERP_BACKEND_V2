using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductKhachHang
{
    public int ID { get; set; }

    public string? MaKhachHang { get; set; }

    public string? TenKhachHang { get; set; }

    public string? TenKiHieu { get; set; }

    public string? DiaChi { get; set; }

    public string? Type { get; set; }
}

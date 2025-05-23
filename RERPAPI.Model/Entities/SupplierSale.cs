using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SupplierSale
{
    public int ID { get; set; }

    public string? CodeNCC { get; set; }

    public string? NameNCC { get; set; }

    public string? AddressNCC { get; set; }

    public string? PhoneNCC { get; set; }

    public string? OrdererNCC { get; set; }

    public string? Debt { get; set; }

    public DateTime? NgayUpdate { get; set; }

    public string? NVPhuTrach { get; set; }

    public string? LoaiHangHoa { get; set; }

    public string? Brand { get; set; }

    public string? MaNhom { get; set; }

    public string? TenTiengAnh { get; set; }

    public string? Website { get; set; }

    public string? SoTK { get; set; }

    public string? NganHang { get; set; }

    public string? MaSoThue { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 1:RTC
    /// 2:MVI
    /// 3:APR
    /// 4:YONKO
    /// </summary>
    public int? Company { get; set; }

    public string? ShortNameSupplier { get; set; }

    public int? EmployeeID { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsDebt { get; set; }

    public string? FedexAccount { get; set; }

    public string? OriginItem { get; set; }

    public string? BankCharge { get; set; }

    public string? AddressDelivery { get; set; }

    public int? RulePayID { get; set; }

    public string? Description { get; set; }

    public string? RuleIncoterm { get; set; }
}

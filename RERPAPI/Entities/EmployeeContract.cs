using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeContract
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? EmployeeID { get; set; }

    public int? EmployeeLoaiHDLDID { get; set; }

    /// <summary>
    /// Ngày bắt đầu hợp đồng
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Ngày kết thúc hợp đồng
    /// </summary>
    public DateTime? DateEnd { get; set; }

    public string? ContractNumber { get; set; }

    /// <summary>
    /// 1: chưa ký; 2: đã ký
    /// </summary>
    public int? StatusSign { get; set; }

    public DateTime? DateSign { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}

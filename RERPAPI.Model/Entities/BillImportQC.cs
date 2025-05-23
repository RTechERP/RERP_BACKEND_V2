using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImportQC
{
    public int ID { get; set; }

    /// <summary>
    /// Nhân viên yêu cầu
    /// </summary>
    public int? EmployeeRequestID { get; set; }

    /// <summary>
    /// Mã billimport
    /// </summary>
    public string? RequestImportCode { get; set; }

    /// <summary>
    /// Ngày yêu cầu QC
    /// </summary>
    public DateTime? RequestDateQC { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? Dealine { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class RegisterContract
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? EmployeeReciveID { get; set; }

    public int? TaxCompanyID { get; set; }

    public DateTime? RegistedDate { get; set; }

    public int? DocumentTypeID { get; set; }

    public string? DocumentName { get; set; }

    public int? DocumentQuantity { get; set; }

    /// <summary>
    /// 1: Sao y, 2: Gốc, 3: Treo
    /// </summary>
    public int? ContractTypeID { get; set; }

    public string? ReasonCancel { get; set; }

    /// <summary>
    /// 0: Chưa nhận,1: Đã nhận, 2: Hủy
    /// </summary>
    public int? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DateApproved { get; set; }

    public bool? IsScan { get; set; }

    public string? FolderPath { get; set; }
}

using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRHiringRequest
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? DepartmentID { get; set; }

    public string? HiringRequestCode { get; set; }

    public int? EmployeeChucVuHDID { get; set; }

    public int? QuantityHiring { get; set; }

    public int? AgeMin { get; set; }

    public int? AgeMax { get; set; }

    public DateTime? DateRequest { get; set; }

    public int? SalaryMin { get; set; }

    public int? SalaryMax { get; set; }

    public int? EmployeeRequestID { get; set; }

    public string? WorkAddress { get; set; }

    public string? ProfessionalRequirement { get; set; }

    public string? JobDescription { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}

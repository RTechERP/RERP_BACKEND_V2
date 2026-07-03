using System;
using System.Collections.Generic;

namespace RERPAPI.Model.DTO;

public class BusinessVisaRequestDto
{
    public int ID { get; set; }
    public int? STT { get; set; }
    public int? Type { get; set; }
    public int? EmployeeID { get; set; }
    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public string? Nation { get; set; }
    public string? HoChieu { get; set; }
    public string? NgheNghiep { get; set; }
    public string? CompanyName { get; set; }
    public string? Destination { get; set; }
    public DateTime? BusinessTripFromDate { get; set; }
    public DateTime? BusinessTripToDate { get; set; }
    public decimal? Cost { get; set; }
    public string? VisaIssueDate { get; set; }
    public string? Note { get; set; }
    public int? Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }

    // Custom fields for UI display
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string? DepartmentName { get; set; }
}

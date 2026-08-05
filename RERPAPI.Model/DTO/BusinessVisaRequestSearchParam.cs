using System;

namespace RERPAPI.Model.DTO;

public class BusinessVisaRequestSearchParam
{
    public string? Keyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Type { get; set; }
    public int? EmployeeID { get; set; }
}

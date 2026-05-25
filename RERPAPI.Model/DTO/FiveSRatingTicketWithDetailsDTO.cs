using System.Collections.Generic;

namespace RERPAPI.Model.DTO;

/// <summary>
/// DTO for creating a ticket with pre-initialized detail rows
/// </summary>
public class FiveSRatingTicketWithDetailsDTO
{
    public int ID { get; set; }
    public int? Rating5SID { get; set; }
    public int? EmployeeRating1ID { get; set; }
    public int? EmployeeRating2ID { get; set; }
    public string? Note { get; set; }
    public List<int> DepartmentIDs { get; set; } = new();
}

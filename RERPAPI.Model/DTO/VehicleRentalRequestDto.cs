using RERPAPI.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.DTO
{
    [NotMapped]
    public class VehicleRentalRequestDto : VehicleRentalRequest
    {
        public string? EmployeeRequestName { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? ProjectName { get; set; }
    }
}

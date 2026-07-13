using System;

namespace RERPAPI.Model.DTO
{
    public class VehicleRentalRequestSearchParam
    {
        public string Keyword { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int EmployeeRequestID { get; set; } = 0;
        public int EmployeeID { get; set; } = 0;
        public int DepartmentID { get; set; } = 0;
    }
}

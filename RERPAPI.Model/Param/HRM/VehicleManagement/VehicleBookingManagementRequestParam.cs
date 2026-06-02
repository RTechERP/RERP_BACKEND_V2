namespace RERPAPI.Model.Param.HRM.VehicleManagement
{
    public class VehicleBookingManagementRequestParam
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Category { get; set; }
        public bool? IsCancel { get; set; }
        public int? Status { get; set; }
        public string? Keyword { get; set; }

        public int? EmployeeId { get; set; }
        public int? DriverEmployeeId { get; set; }
    }
}
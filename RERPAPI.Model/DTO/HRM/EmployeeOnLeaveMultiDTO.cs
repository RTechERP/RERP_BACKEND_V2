using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class EmployeeOnLeaveMultiDTO
    {
        public EmployeeOnLeavePhase Phase { get; set; }
        public List<EmployeeOnLeave> Details { get; set; }
        public bool? IsPartialUpdate { get; set; }
    }
}
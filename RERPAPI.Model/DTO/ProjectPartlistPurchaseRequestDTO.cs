using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProjectPartlistPurchaseRequestDTO : ProjectPartlistPurchaseRequest
    {
        public int? PONCCID { get; set; }
        public int? ProjectID { get; set; }
        public int? CustomerID { get; set; }
        public int? EmployeeBuyID { get; set; }
        public int? JobRequirementEmployeeID { get; set; }
        public int? JobRequirementApprovedTBPID { get; set; }
        public string? TT { get; set; }
        public string? Manufacturer { get; set; }
        public string? ProductNewCode { get; set; }
        public string? UpdatedName { get; set; }
        public bool IsMarketing { get; set; }
        
    }
}

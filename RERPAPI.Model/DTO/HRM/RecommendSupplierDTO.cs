using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class RecommendSupplierDTO
    {
        //public JobRequirement JobRequirement { get; set; }
        public int JobRequirementID { get; set; }

        public List<DepartmentRequired>? DepartmentRequired { get; set; }
        public List<HCNSProposal>? HCNSProposal { get; set; }
        public List<int>? DeletedCommend { get; set; }
        //public List<DepartmentRequiredApproval>? DepartmentRequiredApproval { get; set; }
    }
}
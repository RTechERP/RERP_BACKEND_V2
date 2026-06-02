using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class JobRequirementDTO : JobRequirement
    {
        public List<JobRequirementDetail> JobRequirementDetails { get; set; } = new List<JobRequirementDetail>();
        public List<JobRequirementFile> JobRequirementFiles { get; set; } = new List<JobRequirementFile>();
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM
{
    public class JobRequirementRecommendDTO
    {
        public JobRequirementRecommend Master { get; set; } = new JobRequirementRecommend();
        public List<JobRequirementRecommendDetail> Details { get; set; } = new List<JobRequirementRecommendDetail>();
        public List<int>? DeletedDetailIDs { get; set; }
    }
}
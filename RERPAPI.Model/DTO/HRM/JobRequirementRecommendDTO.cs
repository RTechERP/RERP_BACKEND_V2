using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class JobRequirementRecommendDTO
    {
        public JobRequirementRecommend Master { get; set; } = new JobRequirementRecommend();
        public List<JobRequirementRecommendDetail> Details { get; set; } = new List<JobRequirementRecommendDetail>();
        public List<int>? DeletedDetailIDs { get; set; }
    }
}

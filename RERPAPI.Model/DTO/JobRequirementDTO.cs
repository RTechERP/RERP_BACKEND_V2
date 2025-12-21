using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class JobRequirementDTO: JobRequirement
    {
        public List<JobRequirementDetail> JobRequirementDetails { get; set; } = new List<JobRequirementDetail>();
        public List<JobRequirementFile> JobRequirementFiles { get; set; } = new List<JobRequirementFile>();
    }
}

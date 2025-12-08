using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

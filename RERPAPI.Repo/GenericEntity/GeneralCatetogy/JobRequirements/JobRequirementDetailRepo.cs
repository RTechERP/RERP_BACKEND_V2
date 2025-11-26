using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements
{
    public class JobRequirementDetailRepo : GenericRepo<JobRequirementDetail>
    {
        public JobRequirementDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

    }
}

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment
{
    public class JobPerfomanceEvaluationApproveRepo : GenericRepo<JobPerfomanceEvaluationApprove>
    {
        public JobPerfomanceEvaluationApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

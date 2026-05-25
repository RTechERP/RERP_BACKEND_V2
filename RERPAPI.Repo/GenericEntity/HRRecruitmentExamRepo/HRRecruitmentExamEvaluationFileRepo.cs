using RERPAPI.Model.Entities;
using RERPAPI.Model.Common;
using System;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamEvaluationFileRepo : GenericRepo<HRRecruitmentExamEvaluationFile>
    {
        public HRRecruitmentExamEvaluationFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

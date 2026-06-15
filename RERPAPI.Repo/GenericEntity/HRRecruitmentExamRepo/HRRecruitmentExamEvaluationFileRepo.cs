using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamEvaluationFileRepo : GenericRepo<HRRecruitmentExamEvaluationFile>
    {
        public HRRecruitmentExamEvaluationFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
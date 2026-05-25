using System.Collections.Generic;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class GradeEssayRequestDTO
    {
        public int ExamResultDetailID { get; set; }
        public decimal Score { get; set; }
        public List<HRRecruitmentExamEvaluationFile>? EvaluationFiles { get; set; }
    }

    public class FinalizeGradingRequestDTO
    {
        public int ExamResultID { get; set; }
    }

    public class EvaluateCandidateRequestDTO
    {
        public int HRRecruitmentCandidateID { get; set; }
        public int Status { get; set; }
    }
}

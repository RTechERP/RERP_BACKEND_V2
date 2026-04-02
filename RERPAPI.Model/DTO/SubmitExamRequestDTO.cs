using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class SubmitExamRequestDTO
    {
        public int ExamResultID { get; set; }
        public int? hRRecruitmentCandidateID { get; set; }
        public List<SaveMultiAnswerRequestDTO> Answers { get; set; }
    }
}

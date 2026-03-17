using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class SubmitExamRequestDTO
    {
        public int ExamResultID { get; set; }
        public List<SaveMultiAnswerRequestDTO> Answers { get; set; }
    }
}

using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class HRRecruitmentQuestionAnswersAndRightDTO : HRRecruitmentAnswer
    {
        public bool IsRightAnswer { get; set; }
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class SaveHRRecruitmentQuestionDTO
    {
        public HRRecruitmentQuestion question { get; set; }
        public List<HRRecruitmentQuestionImage> litsQuestionImage { get; set; }
        public List<int> listImageIDDelete { get; set; }
    }
}
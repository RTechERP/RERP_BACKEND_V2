using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class HRRecruitmentQuestionAnswersDTO
    {
        public HRRecruitmentQuestion? question { get; set; }
        public List<HRRecruitmentQuestionAnswersAndRightDTO>? answers { get; set; }
        public List<HRRecruitmentRightAnswer>? rightAnswers { get; set; }
        public List<int>? listAnswerIDDelete { get; set; }
        public int? ExamType { get; set; }
        public List<HRRecruitmentQuestionImage>? litsQuestionImage { get; set; }
        public List<int>? listImageIDDelete { get; set; }
    }
}

using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class SaveMultiAnswerRequestDTO
    {
        public int RecruitmentExamResultID { get; set; }
        public int RecruitmentQuestionID { get; set; }
        public List<int> RecruitmentAnswerIDs { get; set; } // Danh sách các ID đáp án được chọn
        public string AnswerText { get; set; }
        public List<HRRecruitmentExamResultImage>? litsAnswerImage { get; set; }
    }
}

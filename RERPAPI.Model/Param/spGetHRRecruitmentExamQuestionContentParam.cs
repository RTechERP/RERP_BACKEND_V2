using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class spGetHRRecruitmentExamQuestionContentParam
    {
        public int? ExamType { get; set; }
        public int? HRRecruitmentExamID { get; set; }
        public string? FilterText { get; set; }
    }
}

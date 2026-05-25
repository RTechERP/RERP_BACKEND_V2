using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class CopyQuestionAnswersParam
    {
        public List<int>? ListQuestionID { get; set; }
        public int? HRRecruitmentExamID { get; set; }
    }
}

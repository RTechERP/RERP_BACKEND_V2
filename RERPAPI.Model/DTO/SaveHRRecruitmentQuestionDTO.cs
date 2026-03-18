using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class SaveHRRecruitmentQuestionDTO
    {
        public HRRecruitmentQuestion question { get; set; }
        public List<HRRecruitmentQuestionImage> litsQuestionImage { get; set; }
        public List<int> listImageIDDelete { get; set; }
    }
}

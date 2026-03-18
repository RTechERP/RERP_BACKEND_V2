using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentQuestionImageRepo: GenericRepo<HRRecruitmentQuestionImage>
    {
        public HRRecruitmentQuestionImageRepo(CurrentUser currentUser) : base(currentUser)
    {
    }
}
}

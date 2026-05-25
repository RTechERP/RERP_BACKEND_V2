using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamResultRepo : GenericRepo<HRRecruitmentExamResult>
    {
        public HRRecruitmentExamResultRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

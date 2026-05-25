using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentRightAnswearsRepo : GenericRepo<HRRecruitmentRightAnswer>
    {
        public HRRecruitmentRightAnswearsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

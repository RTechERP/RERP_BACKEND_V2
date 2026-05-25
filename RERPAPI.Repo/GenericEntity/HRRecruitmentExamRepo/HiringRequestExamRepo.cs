using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HiringRequestExamRepo : GenericRepo<HRHiringRequestExam>
    {
        public HiringRequestExamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

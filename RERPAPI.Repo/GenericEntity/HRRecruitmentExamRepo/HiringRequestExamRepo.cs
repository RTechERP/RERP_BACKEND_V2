using RERPAPI.Model.DTO;
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
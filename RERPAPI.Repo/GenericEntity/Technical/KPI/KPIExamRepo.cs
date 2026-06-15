using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical.KPI
{
    public class KPIExamRepo : GenericRepo<KPIExam>
    {
        public KPIExamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
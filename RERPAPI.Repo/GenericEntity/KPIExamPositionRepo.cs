using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPIExamPositionRepo : GenericRepo<KPIExamPosition>
    {
        public KPIExamPositionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
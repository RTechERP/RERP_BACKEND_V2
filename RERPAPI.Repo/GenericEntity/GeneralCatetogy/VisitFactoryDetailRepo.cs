using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class VisitFactoryDetailRepo : GenericRepo<VisitFactoryDetail>
    {
        public VisitFactoryDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

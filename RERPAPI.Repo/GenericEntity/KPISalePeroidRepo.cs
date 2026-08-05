using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPISalePeroidRepo : GenericRepo<KPISalePeriod>
    {
        public KPISalePeroidRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
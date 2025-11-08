using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverSubordinateRepo : GenericRepo<HandoverSubordinate>
    {
        public HandoverSubordinateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

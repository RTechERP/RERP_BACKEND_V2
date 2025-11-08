using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverRepo : GenericRepo<Handover>
    {
        public HandoverRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class VisitGuestTypeRepo : GenericRepo<VisitGuestType>
    {
        public VisitGuestTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

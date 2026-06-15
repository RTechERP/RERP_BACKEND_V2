using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class NotificationTypeLinkRepo : GenericRepo<NotificationTypeLink>
    {
        public NotificationTypeLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
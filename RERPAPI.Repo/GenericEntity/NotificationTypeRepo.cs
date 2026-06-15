using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class NotificationTypeRepo : GenericRepo<NotificationType>
    {
        public NotificationTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
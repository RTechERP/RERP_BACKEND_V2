using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class ConfigNotificationKeyLinkRepo : GenericRepo<ConfigNotificationKeyLink>
    {
        public ConfigNotificationKeyLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class UserGroupLinkRepo : GenericRepo<UserGroupLink>
    {
        public UserGroupLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
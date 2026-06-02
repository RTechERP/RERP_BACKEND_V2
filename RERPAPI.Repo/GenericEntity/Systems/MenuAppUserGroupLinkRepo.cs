using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class MenuAppUserGroupLinkRepo : GenericRepo<MenuAppUserGroupLink>
    {
        public MenuAppUserGroupLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UserTeamLinkRepo : GenericRepo<UserTeamLink>
    {
        public UserTeamLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
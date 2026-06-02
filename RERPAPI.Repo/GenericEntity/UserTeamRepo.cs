using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UserTeamRepo : GenericRepo<UserTeam>
    {
        public UserTeamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
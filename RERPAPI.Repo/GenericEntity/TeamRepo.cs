using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TeamRepo : GenericRepo<Team>
    {
        public TeamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
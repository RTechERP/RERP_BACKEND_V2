using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FollowProjectRepo : GenericRepo<FollowProject>
    {
        public FollowProjectRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
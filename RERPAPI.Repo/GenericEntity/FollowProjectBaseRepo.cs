using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FollowProjectBaseRepo : GenericRepo<FollowProjectBase>
    {
        public FollowProjectBaseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
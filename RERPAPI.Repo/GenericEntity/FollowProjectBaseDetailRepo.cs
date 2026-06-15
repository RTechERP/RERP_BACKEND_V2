using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FollowProjectBaseDetailRepo : GenericRepo<FollowProjectBaseDetail>
    {
        public FollowProjectBaseDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class UserGroupRightDistributionRepo : GenericRepo<UserGroupRightDistribution>
    {
        public UserGroupRightDistributionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
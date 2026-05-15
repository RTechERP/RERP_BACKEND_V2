using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class UserGroupRightDistributionRepo : GenericRepo<UserGroupRightDistribution>
    {
        public UserGroupRightDistributionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

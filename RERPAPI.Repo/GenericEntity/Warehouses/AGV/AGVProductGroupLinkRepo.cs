using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVProductGroupLinkRepo : GenericRepo<AGVProductGroupLink>
    {
        public AGVProductGroupLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

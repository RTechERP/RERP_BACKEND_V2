using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVHistoryProductRepo : GenericRepo<AGVHistoryProduct>
    {
        public AGVHistoryProductRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

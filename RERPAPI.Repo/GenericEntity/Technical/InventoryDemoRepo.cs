using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class InventoryDemoRepo : GenericRepo<InventoryDemo>
    {
        public InventoryDemoRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

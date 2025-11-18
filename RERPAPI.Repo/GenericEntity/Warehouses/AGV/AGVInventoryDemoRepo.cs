using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVInventoryDemoRepo : GenericRepo<AGVInventoryDemo>
    {
        public AGVInventoryDemoRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public void SaveData(AGVInventoryDemo inventory)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

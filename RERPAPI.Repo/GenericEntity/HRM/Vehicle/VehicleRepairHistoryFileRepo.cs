using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleRepairHistoryFileRepo : GenericRepo<VehicleRepairHistoryFile>
    {
        public VehicleRepairHistoryFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

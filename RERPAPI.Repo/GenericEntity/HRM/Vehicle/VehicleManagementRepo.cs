using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleManagementRepo : GenericRepo<VehicleManagement>
    {
        public VehicleManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

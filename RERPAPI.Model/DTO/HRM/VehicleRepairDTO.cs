using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class VehicleRepairDTO
    {
     public VehicleRepair? vehicleRepair { set; get; }
     public VehicleRepairType? vehicleRepairType { set; get; }
    }
}

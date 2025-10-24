using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleRepairTypeRepo:GenericRepo<VehicleRepairType>
    {
        public bool Validate(VehicleRepairType item, out string message)
        {
            message = "";

            if (!string.IsNullOrEmpty(item.RepairTypeName) == false)
            {
                message = "Vui lòng nhập tên loại sửa chữa";
                return false;
            }
            return true;
        }
    }
}

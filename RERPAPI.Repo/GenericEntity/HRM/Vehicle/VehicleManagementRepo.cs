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

        public bool Validate(VehicleManagement item, out string message)
        {
            message = "";
            bool exists = GetAll().Any(x => x.LicensePlate == item.LicensePlate && x.ID != item.ID && x.IsDeleted != true&&item.VehicleCategoryID==1);
            if (exists)
            {
                message = $"Phương tiện có biển số {item.LicensePlate} đã tồn tại";
                return false;
            }
            return true;
        }
    }
}

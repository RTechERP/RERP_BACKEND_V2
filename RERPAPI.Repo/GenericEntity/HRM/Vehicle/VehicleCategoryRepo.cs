using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleCategoryRepo : GenericRepo<VehicleCategory>
    {
        public VehicleCategoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool Validate(VehicleCategory item, out string message)
        {
            message = "";
            bool exists = GetAll().Any(x => x.CategoryCode == item.CategoryCode && x.ID != item.ID && x.IsDelete != true);
            if (exists)
            {
                message = $"Mã loại phương tiện {item.CategoryCode} đã tồn tại";
                return false;
            }
            return true;
        }

    }
}

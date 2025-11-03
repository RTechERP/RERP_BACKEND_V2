using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class OfficeSupplyUnitRepo : GenericRepo<OfficeSupplyUnit>
    {
        public bool Validate(OfficeSupplyUnit item, out string message)
        {
            message = "";

          
            bool exists = GetAll().Any(x => x.Name == item.Name && x.ID != item.ID && x.IsDeleted != true);

            if (exists)
            {
                message = "Đơn vị tính  đã tồn tại";
                return false;
            }
            return true;
        }
    }
}

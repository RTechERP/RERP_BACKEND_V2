using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DepartmentRepo : GenericRepo<Department>
    {
        public DepartmentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(Department item, out string message)
        {
            message = "";

            bool exists = GetAll().Any(x => x.Code.ToLower().Trim() == item.Code.ToLower().Trim() && x.ID != item.ID && x.IsDeleted != true);
            if (exists)
            {
                message = "Phòng ban đã tồn tại";
                return false;
            }

            if (String.IsNullOrWhiteSpace(item.Code))
            {
                message = "Mã phòng ban không được để trống";
                return false;
            }
            if (String.IsNullOrWhiteSpace(item.Name))
            {
                message = "Tên phòng ban không được để trống";
                return false;
            }
            if (item.HeadofDepartment <= 0)
            {
                message = "Vui Lòng chọn trưởng phòng";
                return false;
            }


            return true;
        }
    }
}

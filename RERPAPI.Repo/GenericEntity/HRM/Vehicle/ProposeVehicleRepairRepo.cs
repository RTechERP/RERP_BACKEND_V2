using Microsoft.IdentityModel.Tokens;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class ProposeVehicleRepairRepo : GenericRepo<ProposeVehicleRepair>
    {
        public ProposeVehicleRepairRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(ProposeVehicleRepair item, out string message)
        {
            message = "";

            if (item.TimeStartRepair > item.TimeEndRepair)
            {
                message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc";
                return false;
            }
            if (item.DatePropose == null)
            {
                message = "Ngày đề xuất không được để trống";
                return false;
            }
            if (item.EmployeeProposeID == 0 || item.EmployeeProposeID == null)
            {
                message = "Người đề xuất không được để trống";
                return false;
            }
            if (item.ProposeContent == null || item.ProposeContent == "")
            {
                message = "Nội dung đề xuất không được để trống";
                return false;
            }
            if (item.Reason == null || item.Reason == "")
            {
                message = "Lý do đề xuất không được để trống";
                return false;
            }
            if (item.VehicleManagementID == 0 || item.VehicleManagementID == null)
            {
                message = "Chưa chọn phương tiện đề xuất";
                return false;
            }
            if (item.VehicleRepairTypeID == 0 || item.VehicleRepairTypeID == null)
            {
                message = "Chưa chọn loại sửa chữa";
                return false;
            }
            return true;
        }
    }
}

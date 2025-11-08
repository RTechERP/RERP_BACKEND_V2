using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleRepairRepo:GenericRepo<VehicleRepair>
    {
        public VehicleRepairRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(VehicleRepair item, out string message)
        {
            message = "";

            if (!string.IsNullOrEmpty(item.Reason) == false)
            {
                message = "Vui lòng nhập lí do sửa chữa";
                return false;
            }

            if (item.VehicleRepairTypeID != null && item.VehicleRepairTypeID <= 0)
            {
                message = "Vui lòng chọn kiểu sửa chữa";
                return false;
            }

            if (item.TimeStartRepair != null && !item.TimeStartRepair.HasValue)
            {
                message = "Vui lòng nhập thời gian bắt đầu sửa chữa";
                return false;
            }
            if (item.DateReport != null && !item.DateReport.HasValue)

            {
                message = "Vui lòng chọn ngày báo cáo sửa chữa";
                return false;
            }
         

            if (item.CostRepairEstimate != null && !item.CostRepairEstimate.HasValue)
            {
                message = "Vui lòng nhập chi phí ước tính";
                return false;
            }

            return true;
        }

    }
}

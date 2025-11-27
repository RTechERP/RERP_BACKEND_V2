using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class OfficeSupplyRequestsRepo : GenericRepo<OfficeSupplyRequest1>
    {
        public CurrentUser _currentUser;
        public OfficeSupplyRequestsRepo(CurrentUser currentUser) : base(currentUser)
        {
            _currentUser = currentUser;
        }
        public APIResponse Validate(OfficeSupplyRequest1 billExport)
        {
            try
            {
             
                var today = DateTime.Today;
                if (today.Day > 5 && _currentUser.IsAdmin!=true && _currentUser.EmployeeID!=395)
                {
                    return ApiResponseFactory.Fail(null, "Chỉ được đăng ký văn phòng phẩm trước ngày 05 hằng tháng!");
                }

                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}

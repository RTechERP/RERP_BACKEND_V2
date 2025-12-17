using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeOverTimeRepo : GenericRepo<EmployeeOvertime>
    {
        public EmployeeOverTimeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    
        public APIResponse Validate(EmployeeOvertime item)
        {
            try
            {
                bool exists = GetAll().Any(x =>
                    x.ID != item.ID &&
                    x.IsDeleted != true &&
                    x.EmployeeID ==item.EmployeeID&&
                    item.TimeStart < x.EndTime &&
                    item.EndTime > x.TimeStart
                );

                if (exists)
                {
                    return ApiResponseFactory.Fail(
                        null,
                        $"Bạn đã đăng ký làm thêm trong khoảng từ {item.TimeStart} đến {item.EndTime} rồi"
                    );
                }
                if (item.TimeStart >= item.EndTime)
                {
                    return ApiResponseFactory.Fail(
                        null,
                        "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc"
                    );
                }
                if (!item.DateRegister.HasValue)
                {
                    return ApiResponseFactory.Fail(
                        null,
                        "Ngày đăng ký không được để trống"
                    );
                }

                var registerDate = item.DateRegister.Value.Date;
                var time20h = registerDate.AddHours(20);

                if (item.EndTime < time20h&& item.Overnight ==true)
                {
                    return ApiResponseFactory.Fail(
                        null,
                        "Không thể chọn phụ cấp ăn tối trước 20h"
                    );
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

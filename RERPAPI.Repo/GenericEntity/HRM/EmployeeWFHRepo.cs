using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeWFHRepo : GenericRepo<EmployeeWFH>
    {
        public EmployeeWFHRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool Validate(EmployeeWFH item, out string message)
        {
            message = string.Empty;

            if (item == null)
            {
                message = "Dữ liệu WFH không hợp lệ.";
                return false;
            }

            // 1. Bắt buộc chọn người đăng ký
            if (item.EmployeeID == null || item.EmployeeID <= 0)
            {
                message = "Vui lòng nhập Người đăng ký!";
                return false;
            }

            // 2. Bắt buộc chọn người duyệt
            if (item.ApprovedID == null || item.ApprovedID <= 0)
            {
                message = "Vui lòng nhập Người duyệt!";
                return false;
            }

            // 3. Bắt buộc chọn thời gian WFH
            if (item.TimeWFH == null || item.TimeWFH <= 0)
            {
                message = "Vui lòng nhập Thời gian!";
                return false;
            }

            // 3.1 Validate giá trị TimeWFH (1: sáng; 2: chiều; 3: cả ngày)
            if (item.TimeWFH < 1 || item.TimeWFH > 3)
            {
                message = "Thời gian WFH không hợp lệ.";
                return false;
            }

            // 4. Bắt buộc chọn ngày WFH
            if (item.DateWFH == null)
            {
                message = "Vui lòng chọn Ngày WFH!";
                return false;
            }


            var date = item.DateWFH.Value.Date;

            // 5. Check trùng: cùng EmployeeID + DateWFH + TimeWFH, khác ID hiện tại
            var exists = GetAll(x =>
                    x.EmployeeID == item.EmployeeID
                    && x.DateWFH.HasValue
                    && x.DateWFH.Value.Date == date
                    && x.TimeWFH == item.TimeWFH
                    && x.ID != item.ID
                ).Any();

            if (exists)
            {
                message = $"Nhân viên  đã đăng ký WFH cho ngày {item.DateWFH}!";
                return false;
            }

            // 6. Bắt buộc nhập lý do
            if (string.IsNullOrWhiteSpace(item.Reason))
            {
                message = "Vui lòng nhập Lý do!";
                return false;
            }

            // 7. Nếu sửa (ID > 0) thì bắt buộc nhập lý do sửa cho HR
            if (item.ID > 0 && string.IsNullOrWhiteSpace(item.ReasonHREdit))
            {
                message = "Vui lòng nhập Lý do sửa!";
                return false;
            }

            return true;
        }
    }
}

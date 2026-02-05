using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class EmployeeNightShiftRepo : GenericRepo<EmployeeNighShift>
    {
        public EmployeeNightShiftRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool Validate(EmployeeNighShift item, out string message)
        {
            message = string.Empty;

            // 1. Bắt buộc chọn nhân viên
            if (item.EmployeeID == null || item.EmployeeID <= 0)
            {
                message = "Vui lòng chọn nhân viên.";
                return false;
            }

            // 2. Bắt buộc chọn người duyệt
            if (item.ApprovedTBP == null || item.ApprovedTBP <= 0)
            {
                message = "Vui lòng chọn Người duyệt.";
                return false;
            }

            // 3. Bắt buộc nhập ngày đăng ký 
            if (!item.DateRegister.HasValue)
            {
                message = "Vui lòng chọn Ngày đăng ký.";
                return false;
            }

            // 4. Bắt buộc nhập lý do 
            if (string.IsNullOrWhiteSpace(item.Location))
            {
                message = "Vui lòng nhập Lý do.";
                return false;
            }

            // 5. Nếu sửa (ID > 0) thì bắt buộc nhập lý do sửa (ReasonHREdit)
            if (item.ID > 0 && string.IsNullOrWhiteSpace(item.ReasonHREdit))
            {
                message = "Vui lòng nhập Lý do sửa.";
                return false;
            }

            // 6. Check trùng: cùng EmployeeID + DateRegister, khác ID, chưa xóa
            var date = item.DateRegister.Value.Date;

            var exists = GetAll(x =>
                    x.EmployeeID == item.EmployeeID
                    && x.DateRegister.HasValue
                    && x.DateRegister.Value.Date == date
                    && x.ID != item.ID
                    && x.IsDeleted != true
                    && item.DateStart.Value < x.DateEnd.Value
                    && item.DateEnd.Value > x.DateStart.Value
                )
                .Any();
            if (exists)
            {
                message = $"Nhân viên đã khai báo làm đêm ngày [{date:dd/MM/yyyy}]!";
                return false;
            }

            return true;
        }
    }
}

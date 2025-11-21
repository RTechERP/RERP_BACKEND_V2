using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeRepo : GenericRepo<Employee>
    {

        public EmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(Employee item, out string message)
        {
            message = "";
            bool exits = GetAll()
                .Any(x => x.Code.Trim().ToLower() == item.Code.Trim().ToLower()
                    && x.ID != item.ID);
            if (exits)
            {
                message = "Mã nhân viên này đã được sử dụng!";
                return false;
            }

            // ===== 2. Validate Full Name =====
            if (string.IsNullOrWhiteSpace(item.FullName))
            {
                message = "Vui lòng nhập Tên nhân viên!";
                return false;
            }

            // ===== 3. Validate ID chấm công =====
            if (!string.IsNullOrWhiteSpace(item.IDChamCongMoi))
            {
                bool exitsChamCong = GetAll()
                .Any(x => x.IDChamCongMoi.Trim().ToLower() == item.IDChamCongMoi.Trim().ToLower()
                    && x.ID != item.ID);

                if (exitsChamCong)
                {
                    message = "ID chấm công này đã được sử dụng!";
                    return false;
                }
            }

            // ===== 4. Validate Department =====
            if (item.DepartmentID <= 0)
            {
                message = "Vui lòng chọn Phòng ban!";
                return false;
            }

            // ===== 5. Validate Chức vụ HDLD =====
            if (item.ChucVuHDID <= 0)
            {
                message = "Vui lòng chọn Chức vụ (Theo HĐLĐ)!";
                return false;
            }

            // ===== 6. Validate Chức vụ Nội bộ =====
            if (item.ChuVuID <= 0)
            {
                message = "Vui lòng chọn Chức vụ (Theo Nội bộ)!";
                return false;
            }

            // ===== 7. Validate Đơn vị BHXH =====
            if (string.IsNullOrWhiteSpace(item.BHXH))
            {
                message = "Vui lòng chọn Đơn vị bảo hiểm xã hội!";
                return false;
            }

            // ===== 8. Validate Ngày sinh =====
            if (!item.BirthOfDate.HasValue)
            {
                message = "Vui lòng nhập Ngày sinh!";
                return false;
            }

            // ===== 9. Validate Nơi sinh =====
            if (string.IsNullOrWhiteSpace(item.NoiSinh))
            {
                message = "Vui lòng nhập Nơi sinh!";
                return false;
            }

            // ===== 10. Giới tính =====
            if (item.GioiTinh == null)
            {
                message = "Vui lòng chọn Giới tính!";
                return false;
            }

            // ===== 11. Dân tộc =====
            if (string.IsNullOrWhiteSpace(item.DanToc))
            {
                message = "Vui lòng nhập Dân tộc!";
                return false;
            }

            // ===== 12. Tôn giáo =====
            if (string.IsNullOrWhiteSpace(item.TonGiao))
            {
                message = "Vui lòng nhập Tôn giáo!";
                return false;
            }

            // ===== 13. Quốc tịch =====
            if (string.IsNullOrWhiteSpace(item.QuocTich))
            {
                message = "Vui lòng nhập Quốc tịch!";
                return false;
            }

            // ===== 14. Tình trạng hôn nhân =====
            if (item.TinhTrangHonNhanID <= 0)
            {
                message = "Vui lòng chọn Tình trạng hôn nhân!";
                return false;
            }

            // ===== 15. Địa điểm làm việc =====
            if (string.IsNullOrWhiteSpace(item.DiaDiemLamViec))
            {
                message = "Vui lòng nhập Địa điểm làm việc!";
                return false;
            }

            return true;
        }

    }
}

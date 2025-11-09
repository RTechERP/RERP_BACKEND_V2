using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.ProjectAGV;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectRepo : GenericRepo<RERPAPI.Model.Entities.Project>
    {
        public ProjectRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public bool ValidateAGV(RERPAPI.Model.Entities.Project e, out string message)
        {
            message = "";
            int id = e.ID;
            string code = (e.ProjectCode ?? "").Trim();
            string name = (e.ProjectName ?? "").Trim();

            if (string.IsNullOrEmpty(code)) { message = "Vui lòng nhập Mã dự án"; return false; }
            if (string.IsNullOrEmpty(name)) { message = "Vui lòng nhập Tên dự án"; return false; }

            if ((e.ProjectManager ?? 0) <= 0) { message = "Vui lòng chọn PM"; return false; }
            if ((e.ContactID ?? 0) <= 0) { message = "Vui lòng chọn End User/PIC"; return false; }
            if ((e.ProjectType ?? 0) <= 0) { message = "Vui lòng chọn Loại dự án"; return false; }
            if ((e.TypeProject ?? 0) <= 0) { message = "Vui lòng chọn Nhóm dự án"; return false; }
            if ((e.BusinessFieldID ?? 0) <= 0) { message = "Vui lòng chọn Lĩnh vực kinh doanh"; return false; }

            var prio = e.Priotity is decimal d ? d : (e.Priotity ?? 0m);
            if (prio < 0) { message = "Mức ưu tiên không hợp lệ"; return false; }
            if (e.ActualDateStart.HasValue ^ e.ActualDateEnd.HasValue)
            { message = "Vui lòng nhập đủ Ngày bắt đầu và Ngày kết thúc"; return false; }
            if (e.ActualDateStart.HasValue && e.ActualDateEnd.HasValue)
            {
                var s = e.ActualDateStart.Value.Date;
                var t = e.ActualDateEnd.Value.Date;

                if (t < s) { message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu"; return false; }

                if (!IsReasonableDate(s) || !IsReasonableDate(t))
                { message = "Giá trị ngày không hợp lệ (nằm ngoài khoảng 2000-01-01 đến 2100-12-31)"; return false; }
            }
            if (id == 0)
            {
                bool codeExists = GetAll(x => x.ProjectCode == code).Any();
                if (codeExists) { message = $"Mã dự án [{code}] đã tồn tại"; return false; }
            }

            if (id > 0 && GetByID(id) == null)
            { message = $"Không tìm thấy dự án ID={id}"; return false; }

            return true;
        }
        public bool ValidateDTOAGV(ProjectAGVResponseDTO e, out string message)
        {
            message = "";
            int id = e.ID;
            string code = (e.ProjectCode ?? "").Trim();
            string name = (e.ProjectName ?? "").Trim();

            if (string.IsNullOrEmpty(code)) { message = "Vui lòng nhập Mã dự án"; return false; }
            if (string.IsNullOrEmpty(name)) { message = "Vui lòng nhập Tên dự án"; return false; }

            if ((e.ProjectManager ?? 0) <= 0) { message = "Vui lòng chọn PM"; return false; }
            if ((e.ContactID ?? 0) <= 0) { message = "Vui lòng chọn End User/PIC"; return false; }
            if ((e.ProjectType ?? 0) <= 0) { message = "Vui lòng chọn Loại dự án"; return false; }
            if ((e.TypeProject ?? 0) <= 0) { message = "Vui lòng chọn Nhóm dự án"; return false; }
            if ((e.BusinessFieldID ?? 0) <= 0) { message = "Vui lòng chọn Lĩnh vực kinh doanh"; return false; }

            var prio = e.Priotity is decimal d ? d : (e.Priotity ?? 0m);
            if (prio < 0) { message = "Mức ưu tiên không hợp lệ"; return false; }
            if (e.ActualDateStart.HasValue ^ e.ActualDateEnd.HasValue)
            { message = "Vui lòng nhập đủ Ngày bắt đầu và Ngày kết thúc"; return false; }
            if (e.ActualDateStart.HasValue && e.ActualDateEnd.HasValue)
            {
                var s = e.ActualDateStart.Value.Date;
                var t = e.ActualDateEnd.Value.Date;

                if (t < s) { message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu"; return false; }

                if (!IsReasonableDate(s) || !IsReasonableDate(t))
                { message = "Giá trị ngày không hợp lệ (nằm ngoài khoảng 2000-01-01 đến 2100-12-31)"; return false; }
            }
            if (id == 0)
            {
                bool codeExists = GetAll(x => x.ProjectCode == code).Any();
                if (codeExists) { message = $"Mã dự án [{code}] đã tồn tại"; return false; }
            }

            if (id > 0 && GetByID(id) == null)
            { message = $"Không tìm thấy dự án ID={id}"; return false; }

            return true;
        }
        private static bool IsReasonableDate(DateTime d)
        {
            return d >= new DateTime(2000, 1, 1) && d <= new DateTime(2100, 12, 31);
        }


    }
}

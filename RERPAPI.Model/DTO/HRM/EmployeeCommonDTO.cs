using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class EmployeeCommonDTO
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public int Status { get; set; }
        public int ChucVuHDID { get; set; }
        public int ChuVuID { get; set; }
        public Int64 STT { get; set; }
        public string Code { get; set; } = string.Empty;
        public string IDChamCongMoi { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AnhCBNV { get; set; } = string.Empty;

        /// <summary>
        /// Chức vụ Hợp đồng
        /// </summary>
        public string ChucVuHD { get; set; } = string.Empty;

        /// <summary>
        /// Chức vụ nội bộ
        /// </summary>
        public string ChucVu { get; set; } = string.Empty;
        public int DepartmentSTT { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DvBHXH { get; set; } = string.Empty;
        public string DiaDiemLamViec { get; set; } = string.Empty;
        public DateTime? BirthOfDate { get; set; }
        public string NoiSinh { get; set; } = string.Empty;
        public int GioiTinh { get; set; }
        public string GioiTinhText { get; set; } = string.Empty;
        public string DanToc { get; set; } = string.Empty;
        public string TonGiao { get; set; } = string.Empty;
        public string QuocTich { get; set; } = string.Empty;
        public string TinhTrangHonNhan { get; set; } = string.Empty;
        public string DcThuongTru { get; set; } = string.Empty;
        public string DcTamTru { get; set; } = string.Empty;
        public string SDTCaNhan { get; set; } = string.Empty;
        public string EmailCaNhan { get; set; } = string.Empty;
        public string SDTCongTy { get; set; } = string.Empty;
        public string EmailCongTy { get; set; } = string.Empty;
        public string NguoiLienHeKhiCan { get; set; } = string.Empty;
        public string MoiQuanHe { get; set; } = string.Empty;
        public string SDTNguoiThan { get; set; } = string.Empty;
    }
}

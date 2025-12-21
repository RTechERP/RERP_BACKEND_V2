using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportLXCP : GenericRepo<DailyReportHR>
    {
        public DailyReportLXCP(CurrentUser currentUser) : base(currentUser)
        {
        }
        /// <summary>
        /// Validate danh sách báo cáo trước khi lưu
        /// </summary>
        /// <param name="listReport">Danh sách báo cáo</param>
        /// <param name="message">Thông báo lỗi trả về</param>
        /// <param name="employeeID">ID nhân viên đang báo cáo</param>
        /// <returns>True nếu hợp lệ, False nếu có lỗi</returns>
        public bool ValidateDailyReportHRList(List<DailyReportHR> listReport, out string message, int employeeID)
        {
            message = "";
            if (listReport == null || listReport.Count == 0)
            {
                message = "Danh sách báo cáo không được để trống!";
                return false;
            }
            foreach (var item in listReport)
            {
                if (item.ID <= 0) // Chỉ check trùng khi thêm mới
                {
                    // 1. Check trùng lặp ngày báo cáo
                    var exists = this.GetAll().Any(x => x.EmployeeID == employeeID && x.DateReport == item.DateReport && x.IsDeleted ==false);
                    if (exists)
                    {
                        message = $"Bạn đã báo cáo công việc cho ngày {item.DateReport?.ToString("dd/MM/yyyy")}.\nVui lòng kiểm tra lại!";
                        return false;
                    }
                }
                // 2. Validate dữ liệu chi tiết
                if (item.FilmManagementDetailID.HasValue && item.FilmManagementDetailID > 0)
                {
                    // --- Validate CẮT PHIM ---
                    if (item.TimeActual <= 0)
                    {
                        message = "Thời gian thực hiện (Năng suất) phải lớn hơn 0!";
                        return false;
                    }

                    // Nếu cần check Quantity theo cấu hình phim, cần query FilmDetail để lấy cờ RequestResult
                    // Tạm thời check cơ bản:
                    /*
                    if (item.Quantity <= 0) 
                    {
                         message = "Số lượng kết quả phải lớn hơn 0!";
                         return false; 
                    }
                    */
                }
                else
                {
                    // --- Validate LÁI XE ---
                    // Cần có Số Km hoặc Lý do muộn tùy ngữ cảnh
                    if (!item.KmNumber.HasValue)
                    {
                        message = "Vui lòng nhập số Km!";
                        return false;
                    }

                    if ((item.TotalLate.HasValue && item.TotalLate > 0) || (item.TotalTimeLate.HasValue && item.TotalTimeLate > 0))
                    {
                        if (string.IsNullOrEmpty(item.ReasonLate))
                        {
                            message = "Vui lòng nhập Lý do muộn khi có số lần/thời gian muộn!";
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}

using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportHRRepo : GenericRepo<DailyReportTechnical>
    {
        public DailyReportHRRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        /// <summary>
        /// Validate báo cáo công việc hành chính nhân sự (HR)
        /// Dựa trên validate technical nhưng loại bỏ Project, ProjectItem, PercentComplete
        /// </summary>
        public bool ValidateDailyReportHR(
            DailyReportTechnical data,
            out string message,
            List<DailyReportTechnical> existingReports = null,
            int? userId = null)
        {
            message = string.Empty;

            // Kiểm tra null
            if (data == null)
            {
                message = "Không có dữ liệu. Vui lòng kiểm tra lại!";
                return false;
            }

            // Kiểm tra Ngày báo cáo
            if (!data.DateReport.HasValue)
            {
                message = "Vui lòng nhập Ngày báo cáo!";
                return false;
            }

            //// Kiểm tra Tổng số giờ
            //if (data.TotalHours == null || data.TotalHours <= 0)
            //{
            //    message = "Tổng số giờ phải lớn hơn 0!";
            //    return false;
            //}

            //if (data.TotalHours > 24)
            //{
            //    message = "Tổng số giờ không được lớn hơn 24!";
            //    return false;
            //}

            //// Kiểm tra Số giờ OT
            //decimal totalHours = data.TotalHours.Value;
            //decimal totalHourOT = data.TotalHourOT ?? 0;

            //if (totalHourOT < 0)
            //{
            //    message = "Số giờ OT không được nhỏ hơn 0!";
            //    return false;
            //}

            //if (totalHourOT > totalHours)
            //{
            //    message = "Số giờ OT không được lớn hơn Tổng số giờ!";
            //    return false;
            //}

            //// Kiểm tra quy tắc số giờ: Nếu TotalHours > 8 thì phải có TotalHourOT > 0
            //if (totalHours > 8 && totalHourOT <= 0)
            //{
            //    message = "Khi Tổng số giờ lớn hơn 8, vui lòng nhập Số giờ OT!";
            //    return false;
            //}

            //// Kiểm tra: Số giờ hành chính (TotalHours - TotalHourOT) không được > 8
            //if (totalHours - totalHourOT > 8)
            //{
            //    message = "Số giờ hành chính (Tổng số giờ - Số giờ OT) không được lớn hơn 8 giờ!";
            //    return false;
            //}

            // Kiểm tra Nội dung công việc
            if (string.IsNullOrWhiteSpace(data.Content))
            {
                message = "Vui lòng nhập Nội dung công việc!";
                return false;
            }

            //// Kiểm tra nơi làm việc
            //if (string.IsNullOrWhiteSpace(data.Location))
            //{
            //    message = "Vui lòng chọn nơi làm việc!";
            //    return false;
            //}

            // Kiểm tra Kết quả
            if (string.IsNullOrWhiteSpace(data.Results))
            {
                message = "Vui lòng nhập Kết quả!";
                return false;
            }

            // Kiểm tra Kế hoạch ngày tiếp theo
            if (string.IsNullOrWhiteSpace(data.PlanNextDay))
            {
                message = "Vui lòng nhập Kế hoạch ngày tiếp theo!";
                return false;
            }

            // Kiểm tra trùng lặp (nếu có danh sách existing reports và userId)
            // HR không có ProjectItemID nên chỉ kiểm tra trùng DateReport + UserReport
            if (userId.HasValue)
            {
                var duplicates = GetAll().Where(x =>
                    x.ID != data.ID &&
                    x.UserReport == userId.Value &&
                    x.DateReport.HasValue &&
                    data.DateReport.HasValue &&
                    x.DateReport== data.DateReport &&
                    (x.DeleteFlag == null || x.DeleteFlag != 1) &&
                    // HR reports có ProjectID = 0 hoặc null
                    (x.ProjectID == null || x.ProjectID == 0)).ToList();

                // Nếu đã có báo cáo trong ngày và đang thêm mới (ID = 0)
                if (duplicates.Count > 0 && data.ID <= 0)
                {
                    message = $"Bạn đã báo cáo công việc cho ngày {data.DateReport.Value:dd/MM/yyyy}. Vui lòng kiểm tra lại!";
                    return false;
                }
            }

            return true;
        }
    }
}
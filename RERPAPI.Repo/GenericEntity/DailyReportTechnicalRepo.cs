using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class DailyReportTechnicalRepo : GenericRepo<DailyReportTechnical>
    {
        ProjectRepo projectRepo;
        public DailyReportTechnicalRepo(CurrentUser currentUser, ProjectRepo projectRepo) : base(currentUser)
        {
            this.projectRepo = projectRepo;
        }

        public bool ValidateDailyReportTechnical(
            DailyReportTechnical data,
            out string message,
            List<DailyReportTechnical> existingReports = null,
            int? userId = null,
            bool isTechnical = true)
        {
            string projectCode = string.Empty;
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

            // Kiểm tra Dự án (chỉ validate nếu là phòng kỹ thuật)
            if (isTechnical)
            {
                if (data.ProjectID == null || data.ProjectID <= 0)
                {
                    message = "Vui lòng chọn Dự án!";
                    return false;
                }

                // Kiểm tra Hạng mục công việc (nếu có)
                if (data.ProjectItemID != null && data.ProjectItemID <= 0)
                {
                    message = "Vui lòng chọn Hạng mục công việc!";
                    return false;
                }
            }

            // Lấy projectCode để hiển thị trong message lỗi
            if (data.ProjectID.HasValue && data.ProjectID > 0)
            {
                var project = projectRepo.GetByID(data.ProjectID.Value);
                projectCode = project?.ProjectCode ?? string.Empty;
            }

            // Tạo prefix cho message: Dự án [ABC] hoặc để trống nếu không có projectCode
            string projectPrefix = string.IsNullOrEmpty(projectCode)
                ? string.Empty
                : $"Dự án [{projectCode}]";

            // Kiểm tra Tổng số giờ
            if (data.TotalHours == null || data.TotalHours <= 0)
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Tổng số giờ phải lớn hơn 0!"
                    : $"{projectPrefix}: Tổng số giờ phải lớn hơn 0!";
                return false;
            }

            if (data.TotalHours > 24)
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Tổng số giờ không được lớn hơn 24!"
                    : $"{projectPrefix}: Tổng số giờ không được lớn hơn 24!";
                return false;
            }

            // Kiểm tra Số giờ OT
            decimal totalHours = data.TotalHours.Value;
            decimal totalHourOT = data.TotalHourOT ?? 0;

            if (totalHourOT < 0)
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Số giờ OT không được nhỏ hơn 0!"
                    : $"{projectPrefix}: Số giờ OT không được nhỏ hơn 0!";
                return false;
            }

            if (totalHourOT > totalHours)
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Số giờ OT không được lớn hơn Tổng số giờ!"
                    : $"{projectPrefix}: Số giờ OT không được lớn hơn Tổng số giờ!";
                return false;
            }

            // Kiểm tra quy tắc số giờ: Nếu TotalHours > 8 thì phải có TotalHourOT > 0
            if (totalHours > 8 && totalHourOT <= 0)
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Khi Tổng số giờ lớn hơn 8, vui lòng nhập Số giờ OT!"
                    : $"{projectPrefix}: Khi Tổng số giờ lớn hơn 8, vui lòng nhập Số giờ OT!";
                return false;
            }

            // Kiểm tra: Số giờ hành chính (TotalHours - TotalHourOT) không được > 8
            if (totalHours - totalHourOT > 8)
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Số giờ hành chính (Tổng số giờ - Số giờ OT) không được lớn hơn 8 giờ!"
                    : $"{projectPrefix}: Số giờ hành chính (Tổng số giờ - Số giờ OT) không được lớn hơn 8 giờ!";
                return false;
            }

            // Kiểm tra % Hoàn thành (chỉ validate nếu là phòng kỹ thuật)
            if (isTechnical)
            {
                if (data.PercentComplete == null)
                {
                    message = string.IsNullOrEmpty(projectPrefix)
                        ? "Vui lòng nhập % Hoàn thành!"
                        : $"{projectPrefix}: Vui lòng nhập % Hoàn thành!";
                    return false;
                }

                if (data.PercentComplete < 0 || data.PercentComplete > 100)
                {
                    message = string.IsNullOrEmpty(projectPrefix)
                        ? "% Hoàn thành phải từ 0 đến 100!"
                        : $"{projectPrefix}: % Hoàn thành phải từ 0 đến 100!";
                    return false;
                }
            }

            // Kiểm tra Nội dung công việc
            if (string.IsNullOrWhiteSpace(data.Content))
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Vui lòng nhập Nội dung công việc!"
                    : $"{projectPrefix}: Vui lòng nhập Nội dung công việc!";
                return false;
            }

            // Kiểm tra Kết quả
            if (string.IsNullOrWhiteSpace(data.Results))
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Vui lòng nhập Kết quả!"
                    : $"{projectPrefix}: Vui lòng nhập Kết quả!";
                return false;
            }

            // Kiểm tra Kế hoạch ngày tiếp theo
            if (string.IsNullOrWhiteSpace(data.PlanNextDay))
            {
                message = string.IsNullOrEmpty(projectPrefix)
                    ? "Vui lòng nhập Kế hoạch ngày tiếp theo!"
                    : $"{projectPrefix}: Vui lòng nhập Kế hoạch ngày tiếp theo!";
                return false;
            }

            // Kiểm tra trùng lặp (nếu có danh sách existing reports và userId)
            if (existingReports != null && userId.HasValue && isTechnical)
            {
                // Kiểm tra trùng ProjectItemID trong cùng ngày
                if (data.ProjectItemID.HasValue && data.ProjectItemID > 0)
                {
                    var exits = existingReports.Where(x =>
                        x.ID != data.ID &&
                        x.UserReport == userId.Value &&
                        x.DateReport == data.DateReport &&
                        x.ProjectItemID == data.ProjectItemID &&
                        (x.DeleteFlag == null || x.DeleteFlag != 1)).ToList();

                    if (exits.Count > 0)
                    {
                        message = string.IsNullOrEmpty(projectPrefix)
                            ? $"Công việc đã được báo cáo! Vui lòng kiểm tra lại!"
                            : $"{projectPrefix}: Công việc đã được báo cáo! Vui lòng kiểm tra lại!";
                        return false;
                    }
                }
            }

            return true;
        }

        public bool ValidateDailyReportTechnicalList(
           List<DailyReportTechnical> dataList,
           out string message,
           List<DailyReportTechnical> existingReports = null,
           int? userId = null,
           bool isTechnical = true)
        {
            message = string.Empty;

            // Kiểm tra null hoặc empty
            if (dataList == null || dataList.Count == 0)
            {
                message = "Danh sách báo cáo không được rỗng!";
                return false;
            }

            // Tự động lấy existingReports nếu chưa có và có userId
            // Điều này giúp Controller không cần query trước
            if (existingReports == null && userId.HasValue && isTechnical)
            {
                existingReports = GetAll().Where(x =>
                    x.UserReport == userId.Value &&
                    (x.DeleteFlag == null || x.DeleteFlag != 1)).ToList();
            }

            // Validate từng item
            for (int i = 0; i < dataList.Count; i++)
            {
                var item = dataList[i];
                if (!ValidateDailyReportTechnical(item, out string itemMessage, existingReports, userId, isTechnical))
                {
                    message = itemMessage;
                    return false;
                }
            }

            // Validate theo dự án: Tổng số giờ hành chính của tất cả hạng mục trong cùng 1 dự án <= 8
            if (isTechnical && dataList.Count > 1)
            {
                var groupedByProjectAndDate = dataList
                    .Where(x => x.DateReport.HasValue && x.ProjectID.HasValue && x.ProjectID > 0)
                    .GroupBy(x => new {
                        ProjectID = x.ProjectID.Value,
                        DateReport = x.DateReport.Value
                    });

                foreach (var group in groupedByProjectAndDate)
                {
                    // Lấy projectCode
                    var project = projectRepo.GetByID(group.Key.ProjectID);
                    string projectCode = project?.ProjectCode ?? $"ID: {group.Key.ProjectID}";

                    decimal sumTotalHours = group.Sum(x => x.TotalHours ?? 0);
                    decimal sumTotalHourOT = group.Sum(x => x.TotalHourOT ?? 0);
                    decimal totalWorkingHours = sumTotalHours - sumTotalHourOT;

                    if (totalWorkingHours > 8)
                    {
                        message = $"Dự án [{projectCode}]:Tổng [Số giờ] - Tổng [Số giờ OT] trong 1 ngày KHÔNG được lớn hơn 8h.\nVui lòng kiểm tra lại!";
                        return false;
                    }
                }
            }

            // Validate theo ngày: Tổng số giờ hành chính trong toàn bộ ngày <= 8
            if (isTechnical && dataList.Count > 1)
            {
                var groupedByDate = dataList
                    .Where(x => x.DateReport.HasValue)
                    .GroupBy(x => x.DateReport.Value);

                foreach (var group in groupedByDate)
                {
                    decimal sumTotalHours = group.Sum(x => x.TotalHours ?? 0);
                    decimal sumTotalHourOT = group.Sum(x => x.TotalHourOT ?? 0);
                    decimal totalWorkingHours = sumTotalHours - sumTotalHourOT;

                    if (totalWorkingHours > 8)
                    {
                        message = $"Tổng [Số giờ] - Tổng [Số giờ OT] trong 1 ngày KHÔNG được lớn hơn 8h.\nVui lòng kiểm tra lại!";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}


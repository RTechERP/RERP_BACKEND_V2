using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NPOI.HSSF.Record.Chart;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using OfficeOpenXml;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System.Text.RegularExpressions;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyReportTechController : ControllerBase
    {
        DailyReportTechnicalRepo _dailyReportTechnicalRepo;
        ProjectItemRepo _projectItemRepo;
        EmployeeSendEmailRepo _employeeSendEmailRepo;
        private IConfiguration _configuration;
        public DailyReportTechController(DailyReportTechnicalRepo dailyReportTechnicalRepo, ProjectItemRepo projectItemRepo, EmployeeSendEmailRepo employeeSendEmailRepo, IConfiguration configuration)
        {
            _dailyReportTechnicalRepo = dailyReportTechnicalRepo;
            _projectItemRepo = projectItemRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
            _configuration = configuration;
        }
        [HttpPost("get-daily-report-tech")]
        public IActionResult GetDailyReportHr([FromBody] DailyReportTechParam request)
        {
            try
            {

                var keyword = (request.keyword ?? string.Empty).Trim();

                var dataTech = SQLHelper<object>.ProcedureToList(
                    "spGetDailyReportTechnical",
                    new[] { "@DateStart", "@DateEnd", "@TeamID", "@Keyword", "@UserID", "@DepartmentID" },
                    new object[] { request.dateStart ?? DateTime.Now, request.dateEnd ?? DateTime.Now, request.teamID, keyword, request.userID, request.departmentID }
                );
                var technical = SQLHelper<object>.GetListData(dataTech, 0);
                return Ok(ApiResponseFactory.Success(technical,
                    "Lấy dữ liệu thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lay du lieu bao cao theo id 
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetDataByID(int dailyID)
        {
            try
            {
                var dailyData = _dailyReportTechnicalRepo.GetByID(dailyID);
                return Ok(ApiResponseFactory.Success(dailyData, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ham lay hang muc cong viec theo projectId và userID
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="status" -1: lấy tất cả hạng mục ( cho trường hợp sửa) ; 2: lấy các hạng mục chưa hoàn thành (TH thêm mới)></param>
        /// <returns></returns>
        [HttpGet("get-project-item-by-user")]
        public async Task<IActionResult> GetProjectItem(int projectId, int status)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int userId = currentUser.ID;
                var data = SQLHelper<object>.ProcedureToList("spGetProjectItemDetail",
          new string[] { "@ProjectID", "@UserID", "@EmployeeID", "@Status" },
          new object[] { projectId, userId, currentUser.EmployeeID, status });
                return Ok(ApiResponseFactory.Success(SQLHelper<ProjectItem>.GetListData(data, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ham update hang muc cong viec
        [NonAction]
        public async Task UpdateProjectItem(
          DailyReportTechnical request)
        {
            try
            {
                DateTime dateReport = request.DateReport.HasValue ? request.DateReport.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Today;

                DateTime currentDate = DateTime.Now;
                var projectItem = _projectItemRepo.GetByID(request.ProjectItemID ?? 0);
                if (projectItem != null)
                {
                    if (request.PercentComplete == 100)
                    {
                        projectItem.Status = 2; // cập nhật trạng thái hoàn thành 
                        if (!projectItem.ActualStartDate.HasValue)
                        {
                            projectItem.ActualStartDate = dateReport;
                        }
                        if (!projectItem.ActualEndDate.HasValue)
                        {
                            projectItem.ActualEndDate = dateReport;
                            projectItem.UpdatedDateActual = currentDate;
                        }
                    }
                    else
                    {
                        // Nếu % hoàn thành < 100 → Trạng thái đang làm
                        projectItem.Status = 1;
                        if (!projectItem.ActualStartDate.HasValue)
                        {
                            projectItem.ActualStartDate = dateReport;
                        }
                        // Reset ActualEndDate nếu chưa hoàn thành
                        projectItem.ActualEndDate = null;
                        projectItem.UpdatedDateActual = null;
                    }

                    // Cập nhật % hoàn thành thực tế
                    projectItem.PercentageActual = request.PercentComplete;
                    await _projectItemRepo.UpdateAsync(projectItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi bổ sung PO: " + ex.Message);
            }
        }
        [HttpPost("save-report-tech")]
        public async Task<IActionResult> SaveReportTechnical([FromBody] List<DailyReportTechnical> request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int userId = currentUser.ID;

                // 1. Kiểm tra request null hoặc empty
                if (request == null || request.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách báo cáo không được rỗng!"));
                }
                if (!_dailyReportTechnicalRepo.ValidateDailyReportTechnicalList(request, out string validationMessage, existingReports: null, userId, isTechnical: true
               ))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, validationMessage));
                }
                foreach (var item in request)
                {
                    if (item.ID > 0)
                    {
                        await _dailyReportTechnicalRepo.UpdateAsync(item);
                        await UpdateProjectItem(item);
                    }
                    else
                    {
                        // Set các giá trị mặc định
                        item.MasterID = 0;
                        item.Type = 0;
                        item.ReportLate = 0;
                        item.StatusResult = 0;
                        item.Type = 0; // Luôn set Type = 0 (không OT) khi tạo mới
                        item.ReportLate = 0; // Set mặc định = 0, KHÔNG tính toán
                        item.WorkPlanDetailID = 0;
                        item.OldProjectID = 0;
                        item.DeleteFlag = 0;
                        item.Confirm = false;
                        await _dailyReportTechnicalRepo.CreateAsync(item);
                        await UpdateProjectItem(item);

                    }
                }
                return Ok(ApiResponseFactory.Success(null,
                          "Lưu dữ liệu thành công"
                      ));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-daily-report")]
        public async Task<IActionResult> DeletedDailyreport(int dailyReportID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int userId = currentUser.ID;
                DailyReportTechnical data = _dailyReportTechnicalRepo.GetByID(dailyReportID);
                if (data.ID > 0 && data.UserReport == userId)
                {
                    data.DeleteFlag = 1;
                    await _dailyReportTechnicalRepo.UpdateAsync(data);
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể xóa báo cáo của người khác"));
                }
                return Ok(ApiResponseFactory.Success(null, "Đã xóa báo cáo thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-for-copy")]
        public IActionResult GetForCopy([FromBody] DailyReportTechParam request)
        {
            try
            {

                var keyword = (request.keyword ?? string.Empty).Trim();

                var dataTech = SQLHelper<object>.ProcedureToList(
                     "spGetDailyReportTechnicalForCopy",
                    new string[] { "@DateStart", "@DateEnd", "@TeamID", "@Keyword", "@UserID", "@DepartmentID" },
                    new object[] { request.dateStart ?? DateTime.Now, request.dateEnd ?? DateTime.Now, request.teamID, keyword, request.userID, request.departmentID }
                );
                var technical = SQLHelper<object>.GetListData(dataTech, 0);
                return Ok(ApiResponseFactory.Success(technical,
                    "Lấy dữ liệu thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("send-email-report")]
        public async Task<IActionResult> SendEmailReport([FromBody] SendEmailReportRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                // Kiểm tra điều kiện gửi email (chỉ team 10 - có thể điều chỉnh theo yêu cầu)
                if (currentUser.TeamOfUser != 10)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thuộc team được phép gửi email báo cáo."));
                }

                // Validate request
                if (string.IsNullOrWhiteSpace(request.Body))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Nội dung email không được để trống!"));
                }

                // Tạo subject
                var dateReport = request.DateReport ?? DateTime.Now;
                var subject = $"{currentUser.FullName} - BÁO CÁO CÔNG VIỆC NGÀY {dateReport:dd/MM/yyyy}".ToUpper();

                // Chuyển đổi \n thành <br /> nếu body chưa phải HTML
                var emailBody = request.Body;
                if (!emailBody.Contains("<br") && !emailBody.Contains("<div"))
                {
                    emailBody = $"<div>{emailBody.Replace("\n", "<br />")}</div>";
                }
                var email = new
                {
                    Subject = subject,
                    Body = emailBody,
                    //EmailTo = "nguyenvan.thao@rtc.edu.vn", // Email người nhận
                    EmailTo = "nhubinh2104@gmail.com", // Email người nhận        
                    StatusSend = 1, // 1: Đã gửi
                    DateSend = DateTime.Now,
                    EmployeeID = currentUser.EmployeeID,
                    Receiver = 84,
                };
                var emailEntity = new EmployeeSendEmail
                {
                    Subject = email.Subject,
                    Body = email.Body,
                    EmailTo = email.EmailTo,
                    EmailCC = "",
                    StatusSend = email.StatusSend,
                    DateSend = email.DateSend,
                    EmployeeID = email.EmployeeID,
                    Receiver = email.Receiver
                };
                await _employeeSendEmailRepo.CreateAsync(emailEntity);


                return Ok(ApiResponseFactory.Success(null, "Gửi email thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi khi gửi email: {ex.Message}"));
            }
        }

        [HttpPost("export-to-excel")]
        public IActionResult ExportToExcel([FromBody] ExportExcelDailyReportTechRequest request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu request không hợp lệ!"));
                }

                var dateStart = request.DateStart ?? DateTime.Now.AddDays(-30);
                var dateEnd = request.DateEnd ?? DateTime.Now;

                dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);

                var data = SQLHelper<object>.ProcedureToList("spExportToExcelDRT",
                    new string[] { "@DateStart", "@DateEnd", "@TeamID" },
                    new object[] { dateStart, dateEnd, request.TeamID ?? "" });
                var listExport = SQLHelper<Object>.GetListData(data, 0);

                string teamName = request.TeamName ?? "All";
                teamName = Regex.Replace(teamName, @"[^\w\-_\.]", "_");
                string fileName = $"DanhSachBaoCaoCongViec_{teamName}_{dateStart:ddMMyyyy}_{dateEnd:ddMMyyyy}.xlsx";

                string sheetNewName = $"Tháng {dateStart.Month} - {dateEnd.Year}";

                var templateFolder = _configuration.GetValue<string>("PathTemplate");

                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục template trên server!"));

                string templateFileName = _configuration.GetValue<string>("DailyReportTechinial") ?? "DS_DailyReport.xlsx";
                string templatePath = Path.Combine(templateFolder, "ExportExcel", templateFileName);

                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy file mẫu: {templatePath}"));

                // ✅ Set license giống API ExportAllocationAssetReport
                ExcelPackage.License.SetNonCommercialOrganization("RTC");

                using var stream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var package = new ExcelPackage(stream);
                var ws = package.Workbook.Worksheets[0];

                ws.Name = sheetNewName;

                int startRow = 4;

                var firstItem = listExport.Count > 0 ? listExport[0] as IDictionary<string, object> : null;
                var columnKeys = firstItem?.Keys.ToList() ?? new List<string>();
                int colCount = columnKeys.Count;

                for (int i = 0; i < listExport.Count; i++)
                {
                    var row = listExport[i] as IDictionary<string, object>;
                    if (row == null) continue;

                    int currentRow = startRow + i;

                    for (int j = 0; j < columnKeys.Count; j++)
                    {
                        string columnKey = columnKeys[j];
                        var value = row.ContainsKey(columnKey) ? row[columnKey] : null;

                        if (value == null || value == DBNull.Value)
                        {
                            ws.Cells[currentRow, j + 1].Value = "";
                        }
                        else
                        {
                            if (value is DateTime)
                            {
                                ws.Cells[currentRow, j + 1].Value = ((DateTime)value).ToString("dd/MM/yyyy");
                            }
                            else if (value is DateTime?)
                            {
                                var dateValue = (DateTime?)value;
                                ws.Cells[currentRow, j + 1].Value = dateValue.HasValue ? dateValue.Value.ToString("dd/MM/yyyy") : "";
                            }
                            else
                            {
                                ws.Cells[currentRow, j + 1].Value = value.ToString();
                            }
                        }
                    }
                }

                if (listExport.Count > 0)
                {
                    int endRow = startRow + listExport.Count - 1;

                    if (colCount > 0)
                    {
                        ws.Cells[startRow, 1, endRow, colCount].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[startRow, 1, endRow, colCount].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[startRow, 1, endRow, colCount].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[startRow, 1, endRow, colCount].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                }

                var outputStream = new MemoryStream();
                package.SaveAs(outputStream);
                outputStream.Position = 0;

                return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi khi xuất Excel: {ex.Message}"));
            }
        }
    }
}

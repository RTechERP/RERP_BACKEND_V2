using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NPOI.HSSF.Record.Chart;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportTechController : ControllerBase
    {
        DailyReportTechnicalRepo _dailyReportTechnicalRepo;
        ProjectItemRepo _projectItemRepo;
        public DailyReportTechController(DailyReportTechnicalRepo dailyReportTechnicalRepo, ProjectItemRepo projectItemRepo)
        {
            _dailyReportTechnicalRepo = dailyReportTechnicalRepo;
            _projectItemRepo = projectItemRepo;
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
                if(data.ID > 0 && data.UserReport == userId)
                {
                    data.DeleteFlag = 1;
                    await _dailyReportTechnicalRepo.UpdateAsync(data);
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null,"Bạn không thể xóa báo cáo của người khác"));
                }
                return Ok(ApiResponseFactory.Success(null, "Đã xóa báo cáo thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

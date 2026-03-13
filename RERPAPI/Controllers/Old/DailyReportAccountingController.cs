using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity; 
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyReportAccountingController : ControllerBase
    {
        private readonly DailyReportAccountingRepo _dailyReportAccountingRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;

        public DailyReportAccountingController(
            DailyReportAccountingRepo dailyReportAccountingRepo,
            EmployeeRepo employeeRepo,
            vUserGroupLinksRepo vUserGroupLinksRepo)
        {
            _dailyReportAccountingRepo = dailyReportAccountingRepo;
            _employeeRepo = employeeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
        }

        /// <summary>
        /// API lấy danh sách User/Employee để đổ vào Select Box
        /// </summary>
        [HttpGet("get-employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                // Tùy theo logic hiện tại, bạn có thể gọi spGetEmployee qua SQLHelper giống bên Sale 
                // hoặc gọi trực tiếp qua _employeeRepo. Ở đây mình làm cách đơn giản qua Repo.
                var result = _employeeRepo.GetAll(x => x.Status == 0) // Giả sử Status = 0 là đang hoạt động
                    .Select(x => new
                    {
                        ID = x.ID,
                        UserID = x.UserID,
                        FullName = x.FullName,
                        Code = x.Code
                    }).ToList();

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees-by-team-sale")]
        public IActionResult GetEmployeesByTeamSale()
        {
            try
            {
                // Nếu không truyền teamId hoặc teamId = 0 thì lấy full nhân viên bằng spGetEmployee
                //if (teamId == null || teamId == 0)
                //{
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                    var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (x.Code == "N1" || x.Code == "N2" || x.Code == "N60") && x.UserID == currentUser.ID);
                    object data;
                    if (vUserHR == null)
                    {
                        data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                            new string[] { "@Status", "@DepartmentID", "@Keyword" },
                            new object[] { 0, 0, "" });
                    }
                    else
                    {
                        var employee = SQLHelper<object>.ProcedureToList("spGetEmployee",
                            new string[] { "@Status", "@DepartmentID", "@Keyword" },
                            new object[] { 0, 0, "" });
                        data = SQLHelper<object>.GetListData(employee, 0);
                    }
                    return Ok(ApiResponseFactory.Success(data, ""));
                //}

                //// Nếu có teamId thì lấy nhân viên theo team
                //var result =
                //(
                //    from parent in _employeeTeamSaleRepo.GetAll(x => x.ID == teamId && x.IsDeleted != 1)

                //    join child in _employeeTeamSaleRepo.GetAll(x => x.IsDeleted != 1)
                //        on parent.ID equals child.ParentID

                //    join link in _employeeTeamSaleLinkRepo.GetAll()
                //        on child.ID equals link.EmployeeTeamSaleID

                //    join emp in _employeeRepo.GetAll()
                //        on link.EmployeeID equals emp.ID

                //    select new
                //    {
                //        emp.ID,
                //        emp.FullName,
                //        emp.Code,
                //        emp.DepartmentID,
                //        emp.Status,
                //        emp.UserID
                //    }
                //).Distinct().ToList();

                //return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách báo cáo kế toán (có phân trang cơ bản)
        /// </summary>
        //[HttpGet("get-data")]
        //public IActionResult GetData(int page = 1, int size = 20, int? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
        //{
        //    try
        //    {
        //        var query = _dailyReportAccountingRepo.GetAll(x => x.IsDeleted != true).AsQueryable();

        //        if (userId.HasValue && userId > 0)
        //        {
        //            query = query.Where(x => x.UserID == userId.Value);
        //        }

        //        if (fromDate.HasValue)
        //        {
        //            var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
        //            query = query.Where(x => x.ReportDate >= fromDateOnly);
        //        }

        //        if (toDate.HasValue)
        //        {
        //            var toDateOnly = DateOnly.FromDateTime(toDate.Value);
        //            query = query.Where(x => x.ReportDate <= toDateOnly);
        //        }

        //        var totalItem = query.Count();
        //        var data = query.OrderByDescending(x => x.CreatedDate)
        //                        .Skip((page - 1) * size)
        //                        .Take(size)
        //                        .ToList();

        //        var totalPage = (int)Math.Ceiling((double)totalItem / size);

        //        return Ok(ApiResponseFactory.Success(new { data, totalPage }, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpGet("get-data")]
        public async Task<IActionResult> GetData(int page, int size, int? userId, DateTime? dateStart, DateTime? dateEnd, string filterText = "")
        {
            try
            {
                var param = new
                {
                    FilterText = filterText,
                    PageNumber = page,
                    PageSize = size,
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    UserID = userId
                };

                var data = await SqlDapper<object>.ProcedureToListAsync("spGetDailyReportAccounting", param);
                return Ok(ApiResponseFactory.Success(new { data }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết báo cáo theo ID
        /// </summary>
        [HttpGet("get-by-id")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _dailyReportAccountingRepo.GetByID(id);
                if (data == null || data.IsDeleted == true)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
                }

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu hoặc Cập nhật báo cáo
        /// LƯU Ý: Chỗ này truyền thẳng Entity, bạn có thể tạo DailyReportAccountingDTO nếu muốn chặt chẽ hơn
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> Save(List<DailyReportAccountingDTO> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách báo cáo trống"));
                }

                var today = DateTime.Today;
                var minAllowedDate = today.AddDays(-3);

                foreach (var dto in dtos)
                {
                    // Validate
                    if (dto.UserID <= 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "UserID không hợp lệ"));

                    if (dto.ReportDate == default)
                        return BadRequest(ApiResponseFactory.Fail(null, "Ngày báo cáo không hợp lệ"));

                    if (dto.ReportDate.Date < minAllowedDate || dto.ReportDate.Date > today)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null,
                            $"Chỉ được báo cáo trong 3 ngày gần nhất ({minAllowedDate:dd/MM/yyyy} - {today:dd/MM/yyyy})"));
                    }

                    if (string.IsNullOrWhiteSpace(dto.Content))
                        return BadRequest(ApiResponseFactory.Fail(null, "Nội dung công việc không được để trống"));

                    DailyReportAccounting model;

                    if (dto.ID > 0)
                    {
                        model = _dailyReportAccountingRepo.GetByID(dto.ID);

                        if  (model.IsDeleted == true)
                        {
                            return NotFound(ApiResponseFactory.Fail(null, $"Không tìm thấy báo cáo ID = {dto.ID}"));
                        }
                    }
                    else
                    {
                        model = new DailyReportAccounting
                        {
                            IsDeleted = false
                        };
                    }

                    // Mapping trực tiếp
                    model.UserID = dto.UserID;
                    model.ReportDate = dto.ReportDate;
                    model.Content = dto.Content?.Trim();
                    model.Result = dto.Result?.Trim();
                    model.NextPlan = dto.NextPlan?.Trim();
                    model.PendingIssues = dto.PendingIssues?.Trim();
                    model.Urgent = dto.Urgent?.Trim();
                    model.MistakeOrViolation = dto.MistakeOrViolation?.Trim();

                    if (dto.ID > 0)
                    {
                        await _dailyReportAccountingRepo.UpdateAsync(model);
                    }
                    else
                    {
                        await _dailyReportAccountingRepo.CreateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu báo cáo thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa báo cáo (Soft Delete)
        /// </summary>
        [HttpPost("delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var model = _dailyReportAccountingRepo.GetByID(id);
                if (model == null) return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));

                model.IsDeleted = true; // Soft delete

                _dailyReportAccountingRepo.Update(model);

                return Ok(ApiResponseFactory.Success("", "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Asset;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.HRM.OfficeSupplyManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class OfficeSupplyRequestsController : ControllerBase
    {
        private readonly OfficeSupplyRequestsRepo officesupplyrequests;
        OfficeSupplyRequestsDetailRepo _officeSupplyRequestsDetailRepo;

        private readonly DepartmentRepo _departmentRepo;
        private readonly RoleConfig _roleConfig;
        public OfficeSupplyRequestsController(RoleConfig roleConfig,
            OfficeSupplyRequestsRepo officesupplyrequests,
            OfficeSupplyRequestsDetailRepo officeSupplyRequestsDetailRepo,
            DepartmentRepo departmentRepo)
        {
            this.officesupplyrequests = officesupplyrequests;
            _departmentRepo = departmentRepo;
            _roleConfig = roleConfig;
            _officeSupplyRequestsDetailRepo = officeSupplyRequestsDetailRepo;
        }


        [HttpGet("get-data-department")]
        public IActionResult GetdataDepartment()
        {
            try
            {
                //List<Department> departmentList = SQLHelper<Department>.FindAll().OrderBy(x => x.STT).ToList();
                List<Model.Entities.Department> departmentList = _departmentRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(departmentList, ""));
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        /// <summary>
        /// hàm lấy dữ liệu danh sách đăng ký VPP
        /// </summary>
        /// <param name="keyword"></param>
        /// <param id người đăng ký="employeeID"></param>
        /// <param id phòng ban đăng ký="departmentID"></param>
        /// <param tháng="monthInput"></param>
        /// <returns></returns>

        [HttpGet("get-office-supply-request")]
        public IActionResult GetOfficeSupplyRequests(
        string? keyword,
        int? employeeID,
        int? departmentID,
        DateTime? monthInput)
        {
            try
            {
                // Lấy currentUser từ claims
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                keyword ??= string.Empty;
                DateTime month = monthInput ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                bool isPowerUser =
                    (_roleConfig.UserAllsOfficeSupply?.Contains(currentUser.EmployeeID) ?? false) ||
                    (_roleConfig.TBPEmployeeIds?.Contains(currentUser.EmployeeID) ?? false) ||
                    (_roleConfig.PBPPositionCodes?.Contains(currentUser.PositionCode) ?? false) ||
                    currentUser.IsAdmin==true; // nếu có cờ IsAdmin
                bool xemAll = (_roleConfig.UserAllsOfficeSupply?.Contains(currentUser.EmployeeID) ?? false);
                int effectiveDepartmentId;
                int effectiveEmployeeId;

                if (isPowerUser)
                {

                    if (xemAll)
                    {
                        effectiveDepartmentId = 0;
                    }
                    else
                    {
                        effectiveDepartmentId = currentUser.DepartmentID; // 0 = all trong SP
                    }
                    effectiveEmployeeId = employeeID ?? 0;     // 0 = all trong SP
                }
                else
                {
                    // Nhân viên thường:
                    // - Dept: cố định theo phòng ban
                    // - Chỉ xem được phiếu do nó đăng ký
                    effectiveDepartmentId = currentUser.DepartmentID;
                    effectiveEmployeeId = currentUser.EmployeeID;
                }
                if (currentUser.EmployeeID == 331 || currentUser.EmployeeID == 548) //nếu là hương và Hà admin
                {
                    effectiveDepartmentId = currentUser.DepartmentID;
                    effectiveEmployeeId = 0;
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetOfficeSupplyRequests",
                    new string[] { "@KeyWord", "@MonthInput", "@EmployeeID", "@DepartmentID" },
                    new object[] { keyword, month, effectiveEmployeeId, effectiveDepartmentId }
                );

                List<dynamic> rs = result.Count > 0 ? result[0] : new List<dynamic>();

                return Ok(new
                {
                    status = 1,
                    data = rs
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        /// <summary>
        /// Hàm lấy chi tiết đăng ký văn phòng phẩm
        /// </summary>
        /// <param officesupplyrequestsID="id"></param>
        /// <returns></returns>

        [HttpGet("get-office-supply-request-detail")]
        public IActionResult GetOfficeSupplyRequestsDetail(int officeSupplyRequestsID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                        "spGetOfficeSupplyRequestsDetail",
                        new string[] { "@OfficeSupplyRequestsID" },
                       new object[] { officeSupplyRequestsID }

                    );
                List<dynamic> rs = result[0];
                return Ok(new
                {
                    status = 1,
                    data = rs
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("admin-approved")]
        public async Task<IActionResult> adminApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });

                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);

                    if (item != null && item.IsApproved == false && item.IsAdminApproved == false)
                    {
                        item.IsAdminApproved = true;
                        item.DateAdminApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update(item);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Phê duyệt thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("un-admin-approved")]
        public async Task<IActionResult> UnAdminApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });

                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == false)
                    {
                        item.IsAdminApproved = false;
                        item.DateAdminApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update(item);
                }
                return Ok(new
                {
                    status = 1,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }

        [HttpPost("is-approved")]
        public async Task<IActionResult> IsApproved([FromBody] List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });
                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == false)
                    {
                        item.IsApproved = true;
                        item.DateApproved = DateTime.Now;
                        item.ApprovedID = currentUser.EmployeeID;
                    }
                    officesupplyrequests.Update(item);
                }
                return Ok(new
                {
                    status = 1,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("un-is-approved")]
        public IActionResult UnIsApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });
                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == true)
                    {
                        item.IsApproved = false;
                        item.DateApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update(item);
                }
                return Ok(new
                {
                    status = 1,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        /// <summary>
        /// hàm tìm kiếm danh sách tổng đăng ký theo phòng ban
        /// </summary>
        /// <param năm tìm kiếm="year"></param>
        /// <param tháng tìm kiếm="month"></param>
        /// <param tên tìm kiếm="keyword"></param>
        /// <param id phòng ban="departmentId"></param>
        /// <returns></returns>
        [HttpPost("get-office-supply-request-summary")]
        public IActionResult getOfficeSupplyRequestSummary([FromBody] OfficeSupplyRequestSummaryParam filter)
        {
            try
            {
                DateTime dateStart = new DateTime(filter.year, filter.month, 1, 0, 0, 0);
                DateTime dateEnd = dateStart.AddMonths(1).AddSeconds(-1);

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetOfficeSupplyRequestSummary",
                    new string[] { "@DateStart", "@DateEnd", "@Keyword", "@DepartmentID" },
                    new object[] { dateStart, dateEnd, filter.keyword, filter.departmentId }
                );
                return Ok(new
                {
                    status = 1,
                    data = result[0]
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [RequiresPermission("N2,N34,N1,N54,N72")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] OfficeSupplyRequestDTO dto)
            {
            try
            {
             
                if (dto == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
               
                if (dto.officeSupplyRequest != null)
                {
                    var validate = officesupplyrequests.Validate(dto.officeSupplyRequest);
                    if (validate.status == 0) return BadRequest(validate);  
                    if (dto.officeSupplyRequest.ID <= 0)
                        await officesupplyrequests.CreateAsync(dto.officeSupplyRequest);
                    else
                        await officesupplyrequests.UpdateAsync(dto.officeSupplyRequest);
                }

                if (dto.officeSupplyRequestsDetails != null && dto.officeSupplyRequestsDetails.Any())
                {
                    foreach (var item in dto.officeSupplyRequestsDetails)
                    {
                        item.OfficeSupplyRequestsID = dto.officeSupplyRequest.ID;
                        if (item.ID <= 0)
                            await _officeSupplyRequestsDetailRepo.CreateAsync(item);
                        else
                            await _officeSupplyRequestsDetailRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(dto, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

    }
}

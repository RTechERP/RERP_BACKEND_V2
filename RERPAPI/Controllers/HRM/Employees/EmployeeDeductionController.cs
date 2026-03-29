using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeDeductionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly EmployeePayrollDeductionRepo _deductionRepo;

        public EmployeeDeductionController(
            IConfiguration configuration,
            vUserGroupLinksRepo vUserGroupLinksRepo,
            EmployeePayrollDeductionRepo deductionRepo)
        {
            _configuration = configuration;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _deductionRepo = deductionRepo;
        }

        /// <summary>
        /// Get employee deductions with RBAC filtering (N1/N2 see all, others see only their own)
        /// </summary>
        [HttpPost("get-deductions")]
        public IActionResult GetDeductions([FromBody] EmployeeDeductionParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                // Check N1/N2 role
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>
                    (x.Code == "N1" || x.Code == "N2"||currentUser.IsAdmin==true) && x.UserID == currentUser.ID);

                int employeeID;
                int departmentID;
                if (vUserHR != null)
                {
                    // N1/N2: use params from request (can view all)
                    employeeID = param.EmployeeID;
                    departmentID = param.DepartmentID;
                }
                else
                {
                    // Non-N1/N2: can only see their own data
                    employeeID = currentUser.EmployeeID;
                    departmentID = 0;
                }

                var data = SQLHelper<object>.ProcedureToList("spGetEmployeePayrollDeduction",
                    new string[] { "@Month", "@Year", "@EmployeeID", "@DepartmentID", "@Keyword", "@DeductionType" },
                    new object[] { param.Month, param.Year, employeeID, departmentID, param.Keyword ?? "", param.DeductionType });

                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Calculate deductions automatically (N1/N2 only) - calls spInsertIntoEmployeePayrollDeduction
        /// </summary>
        /// 
        [RequiresPermission("N1,N2")]
        [HttpPost("calculate-deductions")]
        public IActionResult CalculateDeductions([FromBody] EmployeeDeductionParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                // Only N1/N2 can calculate
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>
                    (x.Code == "N1" || x.Code == "N2"||currentUser.IsAdmin==true) && x.UserID == currentUser.ID);

                if (vUserHR == null)
                {
                    return Ok(ApiResponseFactory.Unauthorized("Bạn không có quyền thực hiện chức năng này."));
                }

                SQLHelper<object>.ExcuteProcedure("spInsertIntoEmployeePayrollDeduction",
                    new string[] { "@Month", "@Year", "@EmployeeID", "@LoginName", "@IsOverride" },
                    new object[] { param.Month, param.Year, param.EmployeeID, currentUser.Code, param.IsOverride });

                return Ok(ApiResponseFactory.Success(null, "Tính phạt thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Save a manual deduction entry (N1/N2 only)
        /// </summary>
        /// 
        [RequiresPermission("N1,N2")]
        [HttpPost("save-manual")]
        public async Task<IActionResult> SaveManualDeduction([FromBody] EmployeePayrollDeduction model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                // Only N1/N2 can add manual deductions
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>
                    (x.Code == "N1" || x.Code == "N2"||currentUser.IsAdmin==true) && x.UserID == currentUser.ID);

                if (vUserHR == null)
                {
                    return Ok(ApiResponseFactory.Unauthorized("Bạn không có quyền thêm phạt thủ công."));
                }

                if (model.ID == 0)
                {
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = currentUser.LoginName;
                    model.IsDeleted = false;
                 await   _deductionRepo.CreateAsync(model);
                }
                else
                {
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = currentUser.LoginName;
                    await _deductionRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(model, "Lưu thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Get employee deductions summary with RBAC filtering (N1/N2 see all, others see only their own)
        /// </summary>
        [HttpPost("get-deductions-summary")]
        public IActionResult GetDeductionsSummary([FromBody] EmployeeDeductionParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                // Check N1/N2 role
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>
                    (x.Code == "N1" || x.Code == "N2" || currentUser.IsAdmin == true) && x.UserID == currentUser.ID);

                int employeeID;
                int departmentID;
                if (vUserHR != null)
                {
                    // N1/N2: use params from request (can view all)
                    employeeID = param.EmployeeID;
                    departmentID = param.DepartmentID;
                }
                else
                {
                    // Non-N1/N2: can only see their own data
                    employeeID = currentUser.EmployeeID;
                    departmentID = 0;
                }

                var data = SQLHelper<object>.ProcedureToList("spGetEmployeePayrollDeductionSummary",
                    new string[] { "@Month", "@Year", "@EmployeeID", "@DepartmentID", "@Keyword", "@DeductionType" },
                    new object[] { param.Month, param.Year, employeeID, departmentID, param.Keyword ?? "", param.DeductionType });

                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Soft delete a deduction entry (N1/N2 only)
        /// </summary>
        /// 
        [RequiresPermission("N1,N2")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteDeduction([FromBody] EmployeePayrollDeduction param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                // Only N1/N2 can delete
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>
                    (x.Code == "N1" || x.Code == "N2" || currentUser.IsAdmin == true) && x.UserID == currentUser.ID);

                if (vUserHR == null)
                {
                    return Ok(ApiResponseFactory.Unauthorized("Bạn không có quyền thực hiện chức năng này."));
                }

                var model = _deductionRepo.GetByID(param.ID);
                if (model == null)
                {
                    return Ok(ApiResponseFactory.Fail(null,"Dữ liệu không tồn tại."));
                }

                model.IsDeleted = true;
                model.UpdatedBy = currentUser.LoginName;
                model.UpdatedDate = DateTime.Now;
               await _deductionRepo.UpdateAsync(model);

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

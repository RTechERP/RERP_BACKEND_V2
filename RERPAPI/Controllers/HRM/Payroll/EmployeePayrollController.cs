using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePayrollController : Controller
    {
        private readonly EmployeePayrollRepo employeePayrollRepo;
        private readonly EmployeePayrollDetailRepo employeePayrollDetailRepo;
        private readonly DepartmentRepo departmentRepo;
        private readonly EmployeePayrollBonusDeuctionRepo employeePayrollBonusDeuctionRepo;
        private readonly EmployeeRepo employeeRepo;

        public EmployeePayrollController(
            EmployeePayrollRepo employeePayrollRepo,
            EmployeePayrollDetailRepo employeePayrollDetailRepo,
            DepartmentRepo departmentRepo,
            EmployeePayrollBonusDeuctionRepo employeePayrollBonusDeuctionRepo,
            EmployeeRepo employeeRepo)
        {
            this.employeePayrollRepo = employeePayrollRepo;
            this.employeePayrollDetailRepo = employeePayrollDetailRepo;
            this.departmentRepo = departmentRepo;
            this.employeePayrollBonusDeuctionRepo = employeePayrollBonusDeuctionRepo;
            this.employeeRepo = employeeRepo;
        }

        [HttpGet("employee-payroll")]
        public async Task<IActionResult> getemployeepayroll(int year, string? keyWord, int page, int size)
        {
            try
            {
                var payrolls = SQLHelper<object>.ProcedureToList("spGetEmployeePayroll", new string[] { "@Year", "@Keyword", "@PageNumber", "@PageSize" }, new object[] { year, keyWord ?? "", page, size });
                var data = SQLHelper<object>.GetListData(payrolls, 0).OrderBy(c => c._Month);
                var totalPage = data.FirstOrDefault()?.TotalPage ?? 0;

                var result = new
                {
                    data = data,
                    totalPage = totalPage
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employee-payroll-by-id")]
        public async Task<IActionResult> getemployeepayrollbyid(int ID)
        {
            try
            {
                var data = employeePayrollRepo.GetByID(ID);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-exist-employee-payroll")]
        public async Task<IActionResult> getcheckexistemployeepayroll(int id, int month, int year)
        {
            try
            {
                //var data = SQLHelper<object>.ExcuteScalar($"SELECT TOP 1 ID FROM dbo.EmployeePayroll WHERE [_Year] = {year} AND [_Month] = {month} AND ID <> {id}  AND ISNULL(IsDeleted, 0) = 0");
                var data = employeePayrollRepo.GetAll(x=> x._Year == year && x._Month == month && x.ID != id && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employee-payroll-delete-id")]
        public async Task<IActionResult> employeepayrolldeleteid(int ID)
        {
            try
            {
                EmployeePayroll model = employeePayrollRepo.GetByID(ID);
                model.IsDeleted = true;
                employeePayrollRepo.UpdateAsync(model);
                return Ok(ApiResponseFactory.Success("", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("approved-employee-payroll")]
        public async Task<IActionResult> approvedemployeepayroll(int ID, bool Status)
        {
            try
            {
                EmployeePayroll model = employeePayrollRepo.GetByID(ID);
                model.isApproved = Status;
                employeePayrollRepo.UpdateAsync(model);
                return Ok(ApiResponseFactory.Success("", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-employee-payroll")]
        public async Task<IActionResult> saveemployeepayroll([FromBody] EmployeePayroll obj)
        {
            try
            {
                if (obj.ID <= 0) await employeePayrollRepo.CreateAsync(obj);
                else employeePayrollRepo.Update(obj);

                return Ok(ApiResponseFactory.Success(1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}

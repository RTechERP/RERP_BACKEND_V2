using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeErrorController : ControllerBase
    {
        private readonly EmployeeErrorRepo _employeeErrorRepo;
        private readonly EmployeeRepo _employeeRepo;
        public EmployeeErrorController(EmployeeErrorRepo employeeErrorRepo, EmployeeRepo employeeRepo)
        {
            _employeeErrorRepo = employeeErrorRepo;
            _employeeRepo = employeeRepo;

        }

        [HttpGet("get-employee-error")]
        public async Task<IActionResult> getEmployeeError(string? keyword, int pageNumber, int pageSize, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spLoadEmployeeError",
                                                new string[] { @"FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", },
                                                new object[] { keyword ?? "", pageNumber, pageSize, ds, de });

                // Lấy totalPage từ dòng đầu tiên
                int totalPage = 1;
                if (dt != null && dt.Count > 0)
                {
                    var prop = dt[0].GetType().GetProperty("TotalPage");
                    if (prop != null)
                    {
                        totalPage = Convert.ToInt32(prop.GetValue(dt[0], null));
                    }
                }

                return Ok(new
                {
                    status = 1,
                    data = dt,
                    totalPage = totalPage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeError employeeError)
        {
            try
            {
                if (employeeError.ID <= 0) await _employeeErrorRepo.CreateAsync(employeeError);
                else await _employeeErrorRepo.UpdateAsync(employeeError);
                return Ok(ApiResponseFactory.Success(employeeError, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee")]
        public IActionResult GetDepartment()
        {
            try
            {

                var employees = _employeeRepo.GetAll()
                    .Select(x => new
                    {
                        x.ID,
                        x.Code,
                        x.FullName,
                    }).ToList();

                return Ok(new
                {
                    status = 1,
                    data = employees
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }




}

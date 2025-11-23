using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepo _employeeRepo;

        public EmployeeController(EmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        //[HttpGet("employees")]
        //[RequiresPermission("N42")]
        //public IActionResult GetAll()
        //{
        //    try
        //    {
        //        List<Employee> employees = employeeRepo.GetAll();
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = employees
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}


        [HttpGet("employees")]
        public IActionResult GetEmployee(int? status, int? departmentid, string? keyword)
        {
            try
            {
                status = status ?? 0;
                departmentid = departmentid ?? 0;
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                var employees = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentid, keyword });
                var data = SQLHelper<object>.GetListData(employees, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employees/{id}")]
        [RequiresPermission("N42")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Employee employee = _employeeRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        [RequiresPermission("N1,N2,N60")]
        public async Task<IActionResult> SaveEmployee([FromBody] Employee employee)
        {
            try
            {
                if(!_employeeRepo.Validate(employee, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (employee.ID <= 0) await _employeeRepo.CreateAsync(employee);
                else await _employeeRepo.UpdateAsync(employee);

                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

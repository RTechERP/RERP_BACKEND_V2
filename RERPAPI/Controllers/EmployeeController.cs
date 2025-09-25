using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        EmployeeRepo employeeRepo = new EmployeeRepo();

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
        //[RequiresPermission("N42")]
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
        [HttpGet("get-employees")]
        //[RequiresPermission("N42")]
        public IActionResult GetEmployees(int? status)
        {
            try
            {
                status = status ?? 0;
                var employees = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { status });
                return Ok(new
                {
                    status = 1,
                    message = "Success",
                    data = SQLHelper<object>.GetListData(employees, 0)
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

        [HttpGet("employee/{id}")]
        //[RequiresPermission("N42")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Employee employee = employeeRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee.ID <= 0) await employeeRepo.CreateAsync(employee);
                else await employeeRepo.UpdateAsync(employee);

                return Ok(new
                {
                    status = 1,
                    data = employee
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
    }
}

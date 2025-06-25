using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        EmployeeRepo _employeeRepo = new EmployeeRepo();

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                List<Employee> employees = _employeeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = employees
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
        [HttpGet("getemployees")]
        public IActionResult GetEmployee([FromQuery] int status,
                    [FromQuery] int departmentID,
                    [FromQuery] string keyword="")
        {
            try
            {
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                var employees = SQLHelper<EmployeeDTO>.ProcedureToList("spGetEmployee",
                                                                                     new string[] { "@Status", "@DepartmentID", "@Keyword" },                                                                                     new object[] { status, departmentID, keyword });
                return Ok(new
                {
                    status = 1,
                    employees = SQLHelper<dynamic>.GetListData(employees, 0)
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
        [HttpGet("getbyid")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Employee employee = _employeeRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = employee
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

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee.ID <= 0) await _employeeRepo.CreateAsync(employee);
                else await _employeeRepo.UpdateAsync(employee);

                return Ok(new
                {
                    status = 1,
                    data = employee
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
    }
}

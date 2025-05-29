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
        EmployeeRepo employeeRepo = new EmployeeRepo();

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                List<Employee> employees = employeeRepo.GetAll().Where(e=>e.Status!=1).ToList();
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
        public IActionResult GetEmployee(int status, int departmentID,string keyword)
        {
            try
            {
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                var employees = SQLHelper<EmployeeDTO>.ProcedureToList("spGetEmployee", 
                                                                                    new string[] { "@Status", "@DepartmentID", "@Keyword" }, 
                                                                                    new object[] { status, departmentID, keyword });
                return Ok(new
                {
                    status = 1,
                    data = employees[0]
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
                Employee employee = employeeRepo.GetByID(id);
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
        [HttpGet("getbydepartmentid")]
        public IActionResult getEmployeeById(int departmentID)
        {
            List<Employee> employees = employeeRepo.GetAll().Where(e => e.DepartmentID == departmentID).ToList();
            if (employees == null)
            {
                return NotFound(new
                {
                    status = 0,
                    message = "Lỗi không tìm thấy nhân viên của phòng ban"
                });
            }
            return Ok(new
            {
                status = 1,
                data = employees
            });
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

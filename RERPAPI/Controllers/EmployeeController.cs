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
                List<Employee> employees = employeeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = employees
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

        [HttpGet("get-data-employee")]
        public IActionResult GetDataEmployee(int? status, int? departmamentID, string? keyword, int? id)
        {
            try
            {

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetEmployee", new string[] { "@Status", "@DepartmentID", "@Keyword", "@ID" }, new object[] { status ?? 0, departmamentID ?? 0, keyword ?? "", id ?? 0 });
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
                return Ok(new
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

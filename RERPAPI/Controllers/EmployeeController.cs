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
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("getemployees")]
        public IActionResult GetEmployee(int status, int departmentID, string keyword)
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
        [HttpGet("get-all-with-details")]
        public IActionResult GetAllWithDetails()
        {
            try
            {
                var employee = SQLHelper<GetEmployeeDto>.ProcedureToList("GetAllEmployeesWithDetails",
                    new string[] { }, new object[] { });

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

        //[HttpGet("getemployees")]
        //public IActionResult GetEmployee(int status, int departmentID, string keyword)
        //{
        //    try
        //    {
        //        keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
        //        List<EmployeeDTO> employees = SQLHelper<EmployeeDTO>.ProcedureToList("spGetEmployee",
        //                                                                            new string[] { "@Status", "@DepartmentID", "@Keyword" },
        //                                                                            new object[] { status, departmentID, keyword });
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = employees
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}

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

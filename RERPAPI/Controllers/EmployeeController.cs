using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;


//using RERPAPI.Model.Common;
//using RERPAPI.Model.Entities;
//using RERPAPI.Repo.GenericEntity;
using System.Linq;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        EmployeeRepo _employeeRepo = new EmployeeRepo();

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

        [HttpGet("employee/{id}")]
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

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee.ID <= 0) await _employeeRepo.CreateAsync(employee);
                else await _employeeRepo.UpdateAsync(employee);


                //return Ok(new
                //{
                //    status = 1,
                //    data = employee
                //});

                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        public class EmployeeCodeCheck
        {
            public string Code { get; set; }
        }



        [HttpPost("check-codes")]
        public async Task<IActionResult> CheckCodes([FromBody] List<EmployeeCodeCheck> codes)
        {
            try
            {
                var codeList = codes.Select(x => x.Code).ToList();

                // Ki?m tra trong database
                var existingEmployees = _employeeRepo.GetAll()
                    .Where(x => codeList.Contains(x.Code))
                    .Select(x => new
                    {
                        x.ID,
                        x.Code,
                    })
                    .ToList();

                //return Ok(new
                //{
                //    data = new
                //    {
                //        existingEmployees
                //    }
                //});

                return Ok(ApiResponseFactory.Success(new
                {
                    existingEmployees
                }, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new { message = ex.Message });
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
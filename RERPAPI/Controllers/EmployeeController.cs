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
        EmployeeEducationLevelRepo employeeEducationLevelRepo = new EmployeeEducationLevelRepo();

        [HttpGet("get-all")]
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

        [HttpGet]
        public IActionResult GetEmployee(int status, int departmentID,string? keyword)
        {
            try
            {
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                var employees = SQLHelper<object>.ProcedureToList("spGetEmployee", 
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" }, 
                                                new object[] { status, departmentID, keyword });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employees,0)
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

        [HttpGet("{id}")]
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

        [HttpPost]
        public async Task<IActionResult> SaveEmployee([FromBody] EmployeeDTO employee)
        {
            try
            {
                if (employee.ID <= 0) await employeeRepo.CreateAsync(employee);
                else employeeRepo.UpdateFieldsByID(employee.ID, employee);


                if(employee.ID > 0)
                {
                    var existingEductions = SQLHelper<EmployeeEducationLevel>.FindByAttribute("EmployeeID", employee.ID);
                    var educationToDelete = existingEductions.Where(e => !employee.EmployeeEducationLevels.Any(ed => ed.ID == e.ID)).ToList();
                    foreach(var education in educationToDelete)
                    {
                        await employeeEducationLevelRepo.DeleteAsync(education.ID);
                    }
                }

                foreach(var education in employee.EmployeeEducationLevels ?? new List<EmployeeEducationLevel>())
                {
                    var employeeEducation = new EmployeeEducationLevel {
                        ID = education.ID,
                        EmployeeID = employee.ID,
                        RankType = education.RankType,
                        SchoolName = education.SchoolName,
                        TrainType = education.TrainType,
                        Major = education.Major,
                        YearGraduate = education.YearGraduate,
                        Classification = education.Classification
                    };
                    if(employeeEducation.ID <= 0)
                    {
                       await employeeEducationLevelRepo.CreateAsync(employeeEducation);
                    } else
                    {
                        employeeEducationLevelRepo.UpdateFieldsByID(employeeEducation.ID, employeeEducation);
                    }

                }
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

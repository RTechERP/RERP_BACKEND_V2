using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmployeeRepo _employeeRepo;
        vUserGroupLinksRepo _vUserGroupLinksRepo;
        private CurrentUser _currentUser;

        public EmployeeController(EmployeeRepo employeeRepo, vUserGroupLinksRepo vUserGroupLinksRepo, IConfiguration configuration)
        {
            _employeeRepo = employeeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _configuration = configuration;
        }

  

        [HttpGet("employees")]
        public IActionResult GetEmployee(int? status, int? departmentid, string? keyword)
        {
            try
            {
               
                departmentid = departmentid ?? 0;
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                status = status ?? 0;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                object data;
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x =>(x.Code == "N1" || x.Code == "N2" || x.Code == "N60") &&x.UserID == currentUser.ID);
                if (vUserHR == null)
                {
                     data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                 new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                 new object[] { status, departmentid, keyword ?? "" });
                }
                else
                {
                    var employee = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                               new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                               new object[] { status, departmentid, keyword });
                      data = SQLHelper<object>.GetListData(employee, 0);
                }    
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("")]
        public IActionResult GetEmployees(int? status, int? departmentid, string? keyword)
        {
            try
            {
                departmentid = departmentid ?? 0;
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                status = status ?? 0;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                object data;
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var vUserHR = _vUserGroupLinksRepo.GetAll().FirstOrDefault(x => (x.Code == "N1" || x.Code == "N2" || x.Code == "N60") && x.UserID == currentUser.ID);
                if (vUserHR == null)
                {
                    data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentid, keyword ?? "" });
                }
                else
                {
                    var employee = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                               new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                               new object[] { status, departmentid, keyword });
                    data = SQLHelper<object>.GetListData(employee, 0);
                }
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
                RERPAPI.Model.Entities.Employee employee = _employeeRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveEmployee([FromBody] RERPAPI.Model.Entities.Employee employee)
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

        [HttpGet("get-employees")]
        public IActionResult GetEmployees(int? status)
        {
            try
            {
                status = status ?? 0;

                var employees = SQLHelper<object>.ProcedureToList(
                    "spGetEmployee",
                    new string[] { "@Status" },
                    new object[] { status }
                );

                var list = SQLHelper<object>.GetListData(employees, 0)
                    .Select(x => new EmployeeViewPOKHDTO
                    {
                        ID = Convert.ToInt32(x.GetType().GetProperty("ID")?.GetValue(x, null)),
                        FullName = x.GetType().GetProperty("FullName")?.GetValue(x, null)?.ToString()
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

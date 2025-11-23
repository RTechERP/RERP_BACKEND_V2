using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;
using ZXing;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    public class ProjectTypeAssignController : ControllerBase
    {
        ProjectTypeAssignRepo _projectTypeAssignRepo;
        EmployeeRepo _employeeRepo;

        public ProjectTypeAssignController(
            ProjectTypeAssignRepo projectTypeAssignRepo,
            EmployeeRepo employeeRepo
        )
        {
            _projectTypeAssignRepo = projectTypeAssignRepo;
            _employeeRepo = employeeRepo;

        }

        [HttpGet("project-type")]
        [RequiresPermission("N33,N1")]
        public async Task<IActionResult> getProjectType()
        {
            try
            {
                var projectTypes = SQLHelper<object>.ProcedureToList("spGetProjectType",
                    new string[] { "@FilterText" },
                    new object[] { "" });
                var data = SQLHelper<object>.GetListData(projectTypes, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("project-type-assign-by-id")]
        [RequiresPermission("N33,N1")]
        public async Task<IActionResult> getProjectTypeAssign(int projectTypeID)
        {
            try
            {
                var projectTypeAssigns = SQLHelper<object>.ProcedureToList("spGetProjectTypeAssign",
                    new string[] { "ProjectTypeID" },
                    new object[] { projectTypeID });
                var data = SQLHelper<object>.GetListData(projectTypeAssigns, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employees")]
        public IActionResult GetEmployee(int? status, int? departmentId, string? keyword)
        {
            try
            {
                status = status ?? 0;
                departmentId = departmentId ?? 0;
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword;
                var employees = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                new object[] { status, departmentId, keyword ?? "" });


                return Ok(ApiResponseFactory.Success(employees, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("add-employee")]
        [RequiresPermission("N33,N1")]
        public async Task<IActionResult> addEmployees([FromBody] List<int> employeeIds, int projectTypeId)
        {
            try
            {
                if (employeeIds.Count() > 0)
                {
                    foreach (int employeeId in employeeIds)
                    {
                        ProjectTypeAssign assign = _projectTypeAssignRepo.
                            GetAll(x => x.ProjectTypeID == projectTypeId
                            && x.EmployeeID == employeeId)
                            .FirstOrDefault();

                        assign = assign == null ? new ProjectTypeAssign() : assign;
                        assign.ProjectTypeID = projectTypeId;
                        assign.EmployeeID = employeeId;

                        if (assign.ID <= 0)
                        {
                            await _projectTypeAssignRepo.CreateAsync(assign);
                        }
                        else
                        {
                            await _projectTypeAssignRepo.UpdateAsync(assign);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("deleted-project-type-assign")]
        [RequiresPermission("N33,N1")]
        public IActionResult deleteProjectTypeAssigns([FromBody] List<int> projectTypeAssignIds)
        {
            try
            {
                if (projectTypeAssignIds.Count() > 0)
                {
                    foreach (int projectTypeAssignId in projectTypeAssignIds)
                    {
                        _projectTypeAssignRepo.Delete(projectTypeAssignId);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
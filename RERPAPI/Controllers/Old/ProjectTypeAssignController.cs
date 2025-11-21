using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[RequiresPermission("N27,N33,N35,N1")]
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

    }
}
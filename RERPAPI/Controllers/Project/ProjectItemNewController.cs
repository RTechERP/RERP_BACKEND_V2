using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectItemNewController : ControllerBase
    {
        ProjectItemProblemRepo _projectItemProblemRepo = new ProjectItemProblemRepo();
        ProjectItemRepo _projectItemRepo = new ProjectItemRepo();
        [Authorize]
        [HttpGet("get-project-employee-permission")]
        public IActionResult GetProjectEmployeePermission([FromQuery] int? projectID, int? employeeID)
        {
            try
            {
                var rowNum = SQLHelper<dynamic>.ProcedureToList(
             "spGetProjectEmployeePermisstion",
             new string[] { "@ProjectID", "EmployeeID" },
             new object[] { projectID, employeeID }
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        rowNum = SQLHelper<dynamic>.GetListData(rowNum, 0)
                    }
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
        [Authorize]
        [HttpGet("get-project-item")]
        public IActionResult GetProjectItem([FromQuery] int? projectID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
              
                var projectItem = SQLHelper<dynamic>.ProcedureToList(
             "spGetProjectItem",
             new string[] { "@ProjectID", "@UserID", "@Keyword", "@Status" },
             new object[] { projectID, 0, "", "" }
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        projectItem = SQLHelper<dynamic>.GetListData(projectItem, 0)
                    }
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
        [HttpGet("get-project-item-by-item")]
        public IActionResult GetProjectItemByID([FromQuery] int? projectID)
        {
            try
            {
              

                var projectItem = SQLHelper<dynamic>.ProcedureToList(
             "spGetProjectItem",
             new string[] { "@ProjectID", "@UserID", "@Keyword", "@Status" },
             new object[] { projectID, 0, "", "" }
         );
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        projectItem = SQLHelper<dynamic>.GetListData(projectItem, 0)
                    }
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
        [Authorize]
        [HttpGet("get-project-item-code")]
        public async Task<IActionResult> GetProjectItemCode([FromQuery] int projectId)
        {


            string newCode = _projectItemRepo.GenerateProjectItemCode(projectId);
            return Ok(new
            {
                status = 1,
                data = newCode
            });
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectItemFullDTO projectItem)
        {
            try
            {
                //Lưu hạng mục công việc nếu có truyền vào
                if (projectItem.projectItems != null)
                {
                    foreach (var item in projectItem.projectItems)
                    {

                        if (item.ID <= 0)
                            await _projectItemRepo.CreateAsync(item);
                        else
                            _projectItemRepo.UpdateFieldsByID(item.ID, item);
                    }
                }
                // Lưu phát sinh của hạng mục nếu có
                if (projectItem.projectItemProblem != null)
                {
                    if (projectItem.projectItemProblem.ID <= 0)
                        await _projectItemProblemRepo.CreateAsync(projectItem.projectItemProblem);
                    else
                        _projectItemProblemRepo.UpdateFieldsByID(projectItem.projectItemProblem.ID, projectItem.projectItemProblem);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu Thành công",
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

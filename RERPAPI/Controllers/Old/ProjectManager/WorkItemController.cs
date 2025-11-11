using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemController : ControllerBase
    {
        //[HttpGet("get-all/{projectID}")]
        //public IActionResult GetAllWorkItems(int projectID)
        //{
        //    try
        //    {
        //        var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItemDetail",
        //            new[] { "ProjectID" },
        //            new object[] { projectID });
        //        var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
        //        return Ok(ApiResponseFactory.Success(rows, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpGet("get-all/{projectID}")]
        public IActionResult GetAllWorkItems(int projectID)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                    new[] { "ProjectID" },
                    new object[] { projectID });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

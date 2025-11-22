using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Project
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

        //load người phụ trách 
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetUserProjectItem",
                    new[] { "ProjectID" },
                    new object[] { 0 });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load loại dự án 
        [HttpGet("get-type-project-item")]
        public IActionResult GetTypeProject()
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectTypeChildren",
                    new string[] { },
                    new object[] { });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load người giao việc
        [HttpGet("get-employee-request")]
        public IActionResult GetEmployeeRequest()
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeRequestProjectItem",
                    new string[] { },
                    new object[] { });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load người yêu cầu 
        [HttpGet("get-employee-request-name")]
        public IActionResult GetEmployeeRequestName()
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetEmployee",
                    new[] { "@Status" },
                    new object[] { 0 });
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

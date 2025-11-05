using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param.Project;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Old.ProjectManager // tổng hợp phòng ban
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectNewController : ControllerBase
    {
        // Danh sách dự án phòng ban
        [HttpPost("get-projects")]
        public async Task<IActionResult> GetProjects(
            ProjectNewParamRequest filter)
        {
            try
            {
                var result = SQLHelper<object>.ProcedureToList("spGetProjectNew",
                    new string[] {
                        "@DateStart", "@DateEnd", "@DepartmentID", "@UserTeamID", "@UserID", "@ProjectTypeID", "@Keyword"
                    },
                    new object[] {
                        filter.dateTimeS, filter.dateTimeE, filter.departmentID,filter.userTeamID,filter.userID, filter.projectTypeID, filter.keyword
                    });


                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

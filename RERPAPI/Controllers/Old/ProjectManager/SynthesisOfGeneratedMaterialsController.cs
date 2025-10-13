using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class SynthesisOfGeneratedMaterialsController : ControllerBase
    {
        #region Load partList vật tư
        [HttpGet("get-data")]
        public async Task<IActionResult> getData(int pageNumber, int pageSize, DateTime dateStart, DateTime dateEnd,int projectId, string? keyword)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var data = SQLHelper<object>.ProcedureToList("spGetProjectPartlistProblem"
                   , new string[] { "@PageSize", "@PageNumber", "@DateStart", "@DateEnd", "@FilterText", "@ProjectID" }
                   , new object[] { pageSize, pageNumber, ds, de, keyword ?? "", projectId });
                // Lấy từng bảng trong DataSet
                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

    }
}

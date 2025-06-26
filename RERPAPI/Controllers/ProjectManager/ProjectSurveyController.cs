using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectSurveyController : ControllerBase
    {
        #region Khai báo biến
        ProjectRepo projectRepo = new ProjectRepo();
        CustomerRepo customerRepo = new CustomerRepo();
        #endregion

        #region Lấy danh sách tiến độ công việc
        [HttpGet("get-project-survey")]
        public async Task<IActionResult> getProjectSurvey(DateTime dateStart, DateTime dateEnd, int projectId, int technicalId, int saleId, string? keyword)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spGetProjectSurvey",
                                                    new string[] { "@DateStart", "@DateEnd", "@ProjectID", "@EmployeeRequestID", "@EmployeeTechID", "@Keyword" },
                                                    new object[] { ds, de, projectId, saleId, technicalId, keyword ?? "" });
                
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(dt, 0)
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
        #endregion

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkTimelineController : ControllerBase
    {
        #region Khai báo biến
        ProjectRepo projectRepo = new ProjectRepo();
        CustomerRepo customerRepo = new CustomerRepo();
        DepartmentRepo departmentRepo = new DepartmentRepo();
        UserTeamRepo userTeamRepo = new UserTeamRepo();
        #endregion

        #region Lấy danh sách tiến độ công việc
        [HttpGet("get-department")]
        public async Task<IActionResult> getDepartment()
        {
            try
            {
                List<Department> departments = departmentRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data = departments
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

        [HttpGet("get-user-team/{departmentId}")]
        public async Task<IActionResult> getUserTeam(int departmentId)
        {
            try
            {
                List<UserTeam> userTeams = userTeamRepo.GetAll().Where(x => x.DepartmentID == departmentId).ToList();
                return Ok(new
                {
                    status = 1,
                    data = userTeams
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

        [HttpGet("get-data")]
        public async Task<IActionResult> getData(DateTime dateStart, DateTime dateEnd, int departmentId, int userTeamId, int userId, [FromQuery] int[] typeNumber)
        {
            try
            {
                string typeNumberStr = typeNumber != null && typeNumber.Length > 0
                                        ? string.Join(",", typeNumber)
                                        : "";
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var data = SQLHelper<object>.ProcedureToList("spGetTimelineEmployeeWorks"
                                          , new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@UserTeamID", "@UserID", "@IsShowDetail", "@TypeNumber" }
                                          , new object[] { ds, de, departmentId, userTeamId, userId, 0, typeNumberStr });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0)
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

        [HttpGet("get-data-detail")]
        public async Task<IActionResult> getDataDetail(int userId, int type, DateTime date, string typeText, string code)
        {
            try
            {
                string codeNew = code.Replace("\n", "");
                var data = SQLHelper<object>.ProcedureToList("spGetTimelineEmployeeWorksDetail"
                                           , new string[] { "@UserID", "@TypeNumber", "@Date", "@Code" }
                                           , new object[] { userId, type, date, codeNew });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(data, 0)
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

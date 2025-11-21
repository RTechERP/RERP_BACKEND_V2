using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkTimelineController : ControllerBase
    {
        private readonly ProjectRepo projectRepo;
        private readonly CustomerRepo customerRepo;
        private readonly DepartmentRepo departmentRepo;
        private readonly UserTeamRepo userTeamRepo;

        public ProjectWorkTimelineController(
            ProjectRepo projectRepo,
            CustomerRepo customerRepo,
            DepartmentRepo departmentRepo,
            UserTeamRepo userTeamRepo
        )
        {
            this.projectRepo = projectRepo;
            this.customerRepo = customerRepo;
            this.departmentRepo = departmentRepo;
            this.userTeamRepo = userTeamRepo;
        }
        #region Lấy danh sách tiến độ công việc
        [HttpGet("get-department")]
        public async Task<IActionResult> GetDepartment()
        {
            try
            {
                List<Department> departments = departmentRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(departments, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }

        [HttpGet("get-user-team")]
        public async Task<IActionResult> GetUserTeam(int depID)
        {
            try
            {
                List<UserTeam> userTeams = userTeamRepo.GetAll(x => x.DepartmentID == depID);
                return Ok(ApiResponseFactory.Success(userTeams, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }

        [HttpGet("get-data")]
        public async Task<IActionResult> GetData(DateTime dateStart, DateTime dateEnd, int departmentId, int userTeamId, int userId, [FromQuery] int[] typeNumber)
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
                var timeLine = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(timeLine, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }

        [HttpGet("get-data-detail")]
        public async Task<IActionResult> GetDataDetail(int userId, int type, DateTime date, string typeText, string code)
        {
            try
            {
                string codeNew = code.Replace("\n", "");
                var data = SQLHelper<object>.ProcedureToList("spGetTimelineEmployeeWorksDetail"
                                           , new string[] { "@UserID", "@TypeNumber", "@Date", "@Code" }
                                           , new object[] { userId, type, date, codeNew });
                var timeLineDetails = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(timeLineDetails, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion

    }
}

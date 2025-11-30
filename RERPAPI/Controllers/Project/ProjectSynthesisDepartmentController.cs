using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProjectSynthesisDepartmentController : ControllerBase
    {
        private readonly ProjectRepo projectRepo;
        private readonly CustomerRepo customerRepo;
        private readonly DepartmentRepo departmentRepo;
        private readonly UserTeamRepo userTeamRepo;

        public ProjectSynthesisDepartmentController(
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
        #region Load  timeline hạng mục công việc
        [HttpGet("get-data")]
        public async Task<IActionResult> GetData(DateTime dateStart, DateTime dateEnd, int departmentId, int userTeamId, int userId, int projectTypeId, string? keyword)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var data = SQLHelper<object>.ProcedureToList("spGetProjectNew",
                                    new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@UserTeamID", "@UserID", "@ProjectTypeID", "@Keyword" },
                                    new object[] { dateStart, dateEnd, departmentId, userTeamId, userId, projectTypeId, keyword ?? "" });
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

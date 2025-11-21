using DocumentFormat.OpenXml.Bibliography;
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
    public class ProjectWorkItemTimelineController : ControllerBase
    {
        private readonly ProjectRepo projectRepo;
        private readonly CustomerRepo customerRepo;
        private readonly DepartmentRepo departmentRepo;
        private readonly UserTeamRepo userTeamRepo;

        public ProjectWorkItemTimelineController(
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
        public async Task<IActionResult> getData(DateTime dateStart, DateTime dateEnd, int departmentId, int userTeamId, int employeeId, int status)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var data = SQLHelper<object>.ProcedureToList("sp_GetHangMucCongViec",
                                                   new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@TeamID", "@DepartmentID", "@Status" },
                                                   new object[] { ds, de, employeeId, userTeamId, departmentId, status });
                // Lấy từng bảng trong DataSet
                var dt = SQLHelper<object>.GetListData(data, 0);
                var dtAllDate = SQLHelper<object>.GetListData(data, 1);
                var dtMonth = SQLHelper<object>.GetListData(data, 2);

                var result = new
                {
                    dt,
                    dtAllDate,
                    dtMonth
                };
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

        #region lấy danh sách team theo phòng ban
        [HttpGet("get-user-team")]
        public async Task<IActionResult> getUserTeam(int depID)
        {
            try
            {
                var data = userTeamRepo.GetAll(x => x.DepartmentID == depID);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

    }
}

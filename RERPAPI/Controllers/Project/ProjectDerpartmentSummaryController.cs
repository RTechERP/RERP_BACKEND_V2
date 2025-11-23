using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Project;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Project // tổng hợp phòng ban
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectDerpartmentSummaryController : ControllerBase
    {
        private readonly ProjectRepo _projectRepo;
        private readonly ProjectWorkerTypeRepo _projectWorkerTypeRepo;
        public ProjectDerpartmentSummaryController(ProjectRepo projectRepo, ProjectWorkerTypeRepo projectWorkerTypeRepo)
        {
            _projectRepo = projectRepo;
            _projectWorkerTypeRepo = projectWorkerTypeRepo;
        }
        // Danh sách dự án phòng ban
        [HttpPost("get-projects")]
        public async Task<IActionResult> GetProjects(
            ProjectDerpartmentSummaryParamRequest filter)
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
        //danh sách hiện trạng dự án
        [HttpGet("get-project-current-situation/{projectID}")]
        public async Task<IActionResult> GetProjectCurrentSituation(int projectID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetProjectCurrentSituation", new string[] { "@ProjectID" }, new object[] { projectID });
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy chi tiết tổng hợp nhân công
        [HttpPost("get-project-worker-synthetic")]
        public async Task<IActionResult> GetProjectWorkerSynthetic(int projectId, int prjWorkerTypeId, string? keyword)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProjectWokerSynthetic",
                new string[] { "@ProjectID", "@ProjectWorkerTypeID", "@Keyword" },
                new object[] { projectId, prjWorkerTypeId, keyword ?? "" });
                var projectWorker = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(projectWorker, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //lấy dữ liệu combobox dự án 
        [HttpGet("get-combobox-project")]
        public async Task<IActionResult> GetComboboxProject()
        {
            try
            {
                var result = _projectRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //lấy dữ liệu loại nhân công 
        [HttpGet("get-worker-type")]
        public async Task<IActionResult> GetWorkerType()
        {
            try
            {
                var result = _projectWorkerTypeRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

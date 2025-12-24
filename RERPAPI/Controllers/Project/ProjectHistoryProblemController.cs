using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Project
{


    [Route("api/[controller]")]
    [ApiController]
    public class ProjectHistoryProblemController : ControllerBase
    {
        private readonly ProjectHistoryProblemDetailRepo _historyproblemDetailRepo;
        private readonly ProjectHistoryProblemRepo _historyproblemRepo;
        public ProjectHistoryProblemController(
               ProjectHistoryProblemDetailRepo historyproblemDetailRepo, ProjectHistoryProblemRepo historyproblemRepo)
        {
            _historyproblemDetailRepo = historyproblemDetailRepo;
            _historyproblemRepo = historyproblemRepo;
        }
        [HttpPost("get-data")]
        public async Task<IActionResult> getDataHistoryProblem(int projectID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList(
                "spGetProjectHistoryProblemDetail",
                new string[] { "@ProjectID" },
                new object[] { projectID });

                var dtDetail = SQLHelper<object>.GetListData(data, 0);
                var dtMaster = SQLHelper<object>.GetListData(data, 2);
                return Ok(ApiResponseFactory.Success(new { dtDetail, dtMaster },
                    "Lấy dữ liệu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-data-detail")]
        public async Task<IActionResult> getDataHistoryProblemDetail(int id)
        {
            try
            {
                var data = _historyproblemDetailRepo.GetAll(x => x.ProjectHistoryProblemID == id && x.IsDeleted == false);

                return Ok(ApiResponseFactory.Success(
                    data,
                    "Lấy dữ liệu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-problem")]
        public async Task<IActionResult> saveData([FromBody] List<ProjectHistoryProblemDTO> request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (!_historyproblemRepo.Validate(request, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                //lưu problem
                foreach (var item in request)
                {
                    if (item.projectHistoryProblem != null)
                    {
                        if (item.projectHistoryProblem.ID > 0)
                        {
                            //item.projectHistoryProblem.EmployeeID = currentUser.EmployeeID;
                            await _historyproblemRepo.UpdateAsync(item.projectHistoryProblem);
                        }
                        else
                        {
                            item.projectHistoryProblem.EmployeeID = currentUser.EmployeeID;
                            await _historyproblemRepo.CreateAsync(item.projectHistoryProblem);
                        }
                    }
                }
                foreach (var item in request)
                {
                    if (item.detail != null && item.detail.Count > 0)
                    {
                        foreach (var items in item.detail)
                        {
                            if (items.ID > 0)
                            {
                                await _historyproblemDetailRepo.UpdateAsync(items);
                            }
                            else
                            {
                                await _historyproblemDetailRepo.CreateAsync(items);
                            }
                        }
                    }
                }
                //logic xóa dòng ở master, detail
                foreach (var item in request)
                {
                    if (item.deleteIdsMaster != null && item.deleteIdsMaster.Count > 0)
                        foreach (var ids in item.deleteIdsMaster)
                        {
                            ProjectHistoryProblem p = _historyproblemRepo.GetByID(ids);
                            p.IsDeleted = true;
                            await _historyproblemRepo.UpdateAsync(p);
                            List<ProjectHistoryProblemDetail> d = _historyproblemDetailRepo.GetAll(x => x.ProjectHistoryProblemID == ids);
                            if (d != null && d.Count > 0)
                            {
                                foreach (var idD in d)
                                {
                                    idD.IsDeleted = true;
                                    await _historyproblemDetailRepo.UpdateAsync(idD);
                                }
                            }
                        }
                    if (item.deletedIdsDetail != null && item.deletedIdsDetail.Count > 0)
                    {
                        foreach (var ids in item.deletedIdsDetail)
                        {
                            ProjectHistoryProblemDetail p = _historyproblemDetailRepo.GetByID(ids);
                            p.IsDeleted = true;
                            await _historyproblemDetailRepo.UpdateAsync(p);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(
                   null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("add-problem")]
        public async Task<IActionResult> GetProjectHistoryProblemByProject(int projectID, DateTime dateProblem)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var fromDate = dateProblem.Date;
                var toDate = fromDate.AddDays(1);

                var data = _historyproblemRepo.GetAll()
                    .Where(p => p.ProjectID == projectID
                        && p.EmployeeID == currentUser.EmployeeID
                        && p.DateProblem.HasValue
                        && p.DateProblem.Value >= fromDate
                        && p.DateProblem.Value < toDate)
                    .Select(p => new
                    {
                        contentError = p.ContentError,
                        remedies = p.Remedies
                    })
                    .ToList();
                // 3. Trả về đúng danh sách data để JS loop
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

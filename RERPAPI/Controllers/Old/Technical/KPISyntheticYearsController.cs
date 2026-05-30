using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPISyntheticYearsController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        public KPISyntheticYearsController(DepartmentRepo departmentRepo, vUserGroupLinksRepo vUserGroupLinksRepo)
        {
            _departmentRepo = departmentRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
        }
        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0 });

                var vUserGroupLinks = _vUserGroupLinksRepo.GetAll(x => x.UserID == currentUser.ID && x.Code == "N38");
                return Ok(ApiResponseFactory.Success(new { data, vUserGroupLinks }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-data-rule")]
        public async Task<IActionResult> LoadDataRule(int year, int departmentId, int employeeId, string keyword = "")
        {
            try
            {
                var param = new
                {
                    Year = year,
                    DepartmentID = departmentId,
                    EmployeeID = employeeId,
                    Keyword = keyword
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetSyntheticKPI", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

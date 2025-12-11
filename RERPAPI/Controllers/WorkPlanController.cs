using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkPlanController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;
        private readonly WorkPlanRepo _workPlanRepo;
        private readonly WorkPlanDetailRepo _planDetailRepo;

        public WorkPlanController(IConfiguration configuration, CurrentUser currentUser, WorkPlanRepo workPlanRepo, WorkPlanDetailRepo planDetailRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _workPlanRepo = workPlanRepo;
            _planDetailRepo = planDetailRepo;
        }


        [HttpPost("")]
        public IActionResult GetAll([FromBody] WorkPlanParam param)
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                param.DateStart = param.DateStart.Value.Date.AddSeconds(-1);
                param.DateEnd = param.DateEnd.Value.Date.AddDays(+1);

                var data = SQLHelper<object>.ProcedureToList("spGetWorkPlanPaging",
                                            new string[] { "@StartDate", "@EndDate", "@UserID", "@PageNumber", "@PageSize", "@Keyword" },
                                            new object[] { param.DateStart, param.DateEnd, _currentUser.ID, param.PageNumber, param.PageSize, param.Keyword });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0)));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] WorkPlan plan)
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                var validate = _workPlanRepo.Validate(plan);
                if (validate.status == 0)
                {
                    return BadRequest(validate);
                }
                plan.UserID = _currentUser.ID;
                plan.TotalDay = TextUtils.ToInt32((plan.EndDate.Value.Date - plan.StartDate.Value.Date).TotalDays) + 1;
                if (plan.ID <= 0) await _workPlanRepo.CreateAsync(plan);
                else await _workPlanRepo.UpdateAsync(plan);

                //Thêm chi tiết kế hoạch
                await _planDetailRepo.Create(plan);

                return Ok(ApiResponseFactory.Success(plan, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

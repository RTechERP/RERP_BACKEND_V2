using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Asset;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                param.DateStart = param.DateStart.Value.Date.AddSeconds(-1);
                param.DateEnd = param.DateEnd.Value.Date.AddDays(+1);

                var data = SQLHelper<object>.ProcedureToList("spGetWorkPlanPaging",
                                            new string[] { "@StartDate", "@EndDate", "@UserID", "@PageNumber", "@PageSize", "@Keyword" },
                                            new object[] { param.DateStart, param.DateEnd, currentUser.ID, param.PageNumber, param.PageSize, param.Keyword });

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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var validate = _workPlanRepo.Validate(plan);
                if (validate.status == 0)
                {
                    return BadRequest(validate);
                }
                plan.UserID = currentUser.ID;
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

        /// <summary>
        /// Lấy kế hoạch theo ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = _workPlanRepo.GetByID(id);
                if (data == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kế hoạch!"));
                }

                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa kế hoạch theo ID
        /// </summary>
        //[HttpPost("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    { 
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

        //        var workPlan = _workPlanRepo.GetByID(id);
        //        var workPlanDetails = _planDetailRepo.GetAll(x => x.WorkPlanID == id && x.IsDeleted != true).FirstOrDefault();
        //        if (workPlan == null)
        //        {
        //            return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kế hoạch!"));
        //        }

        //        // Kiểm tra quyền xóa - chỉ được xóa kế hoạch của chính mình
        //        if (workPlan.UserID != currentUser.ID)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa kế hoạch này!"));
        //        }
        //        workPlan.IsDeleted=true;
        //        if(workPlanDetails!=null)
        //        {
        //            workPlanDetails.IsDeleted=true;
        //            // Xóa chi tiết kế hoạch trước
        //            await _planDetailRepo.UpdateAsync(workPlanDetails);
        //        }    
        //        // Xóa kế hoạch
        //        await _workPlanRepo.UpdateAsync(workPlan);

        //        return Ok(ApiResponseFactory.Success(null, "Xóa thành công!"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("summary")]
        public IActionResult GetWorkPlanSummary([FromBody] WorkPlanSummaryParam param)
        {
            try
            {
                param.DateStart = param.DateStart.Value.Date.AddSeconds(-1);
                param.DateEnd = param.DateEnd.Value.Date.AddDays(1);

                var departmentId = param.TeamId > 0 ? 0 : param.DepartmentId;

                var data = SQLHelper<object>.ProcedureToList(
                    "spGetWorkPlanDetail1",
                    new string[] { "@DateStart", "@DateEnd", "@FilterText", "@DepartmentID", "@TeamID", "@Type", "@UserID" },
                    new object[] {
                param.DateStart,    
                param.DateEnd,
                param.Keyword ?? "",
                departmentId,
                param.TeamId,
                param.Type,
                param.UserId
                    });

                var dtAllDate = SQLHelper<dynamic>.GetListData(data, 0);
                var dtWorkPlan = SQLHelper<dynamic>.GetListData(data, 1);
                var dtWorkPlanPreCur = SQLHelper<dynamic>.GetListData(data, 2);

                // ===== Build list cột =====
                var columnNames = new List<string> { "Họ tên" };
                foreach (var item in dtAllDate)
                {
                    var date = Convert.ToDateTime(item.AllDates).ToString("yyyy-MM-dd");
                    columnNames.Add(date);
                }

                // ===== Map Pre / Cur =====
                var preCurGroup = dtWorkPlanPreCur
                    .GroupBy(x => (int)x.UserID)
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            Pre = g.Select(x => (string)x.WorkContentPre).Distinct().ToList(),
                            Cur = g.Select(x => (string)x.WorkContentCur).Distinct().ToList()
                        });
                var result = new List<WorkPlanDetailDTO>();

                foreach (var row in dtWorkPlan)
                {
                    var dto = new WorkPlanDetailDTO
                    {
                        FullName = row.FullName
                    };

                    var dict = (IDictionary<string, object>)row;

                    for (int i = 1; i < columnNames.Count; i++)
                    {
                        var key = columnNames[i];
                        var value = dict.ContainsKey(key) ? dict[key]?.ToString() : null;

                        typeof(WorkPlanDetailDTO)
                            .GetProperty($"Day{i}")
                            ?.SetValue(dto, value);
                    }

                    int userId = row.UserID;
                    if (preCurGroup.ContainsKey(userId))
                    {
                        dto.WorkPlanPreviousWeek = string.Join("\n", preCurGroup[userId].Pre).Trim();
                        dto.WorkPlanCurrentWeek = string.Join("\n", preCurGroup[userId].Cur).Trim();
                    }

                    result.Add(dto);
                }

                return Ok(ApiResponseFactory.Success(
                    new
                    {
                        columns = columnNames,
                        data = result
                    },
                    "Lấy dữ liệu thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


     
        [HttpGet("summarize-work")]
        public IActionResult GetSummarizeWork(
            [FromQuery] DateTime dateStart,
            [FromQuery] DateTime dateEnd,
            [FromQuery] int departmentID = 0,
            [FromQuery] int teamID = 0,
            [FromQuery] int userID = 0,
            [FromQuery] string keyWord = "")
        {
            try
            {
            

                var data = SQLHelper<object>.ProcedureToList("spGetSummarizeWork_New",
                    new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@TeamID", "@UserID", "@Keyword" },
                    new object[] { dateStart, dateEnd, departmentID, teamID, userID, keyWord ?? "" });

                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
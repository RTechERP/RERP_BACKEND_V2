using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.VisionBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanWeekController : ControllerBase
    {
        WeekPlanRepo _weekPlanRepo;
        DepartmentRepo _departmentRepo;
        public PlanWeekController(WeekPlanRepo weekPlanRepo,
            DepartmentRepo departmentRepo)
        {
            _weekPlanRepo = weekPlanRepo;
            _departmentRepo = departmentRepo;
        }


        [HttpGet]
        public IActionResult Get(DateTime dateStart, DateTime dateEnd, int departmentId, int userId, int groupSaleId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPlanWeek",
                                        new string[] { "@DateStart", "@DateEnd", "@Department", "@UserID", "@GroupSaleID" },
                                        new object[] { dateStart, dateEnd, departmentId, userId, groupSaleId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                var data1 = SQLHelper<dynamic>.GetListData(list, 1);
                return Ok(ApiResponseFactory.Success(new { data, data1 }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var departments = _departmentRepo.GetAll();
                return Ok(ApiResponseFactory.Success(departments, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-team")]
        public IActionResult GetTeam()
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeManager", new string[] { "@group" }, new object[] { 0 });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Save(List<WeekPlan> model)
        {
            try
            {
                foreach (var item in model)
                {

                    var validate = item;

                    List<string> errors = new List<string>();

                    if (validate.Result?.Length > 500)
                        errors.Add("Kết quả mong đợi không được vượt quá 500 ký tự");
                    if (validate.ContentPlan?.Length > 500)
                        errors.Add("Nội dung không được vượt quá 500 ký tự");


                    if (errors.Any())
                    {
                        var errorMessage = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                        return Ok(ApiResponseFactory.Fail(null, errorMessage, new { Errors = errors }));
                    }

                    WeekPlan weekPlan = item.ID > 0 ? _weekPlanRepo.GetByID(item.ID) : new WeekPlan();
                    weekPlan.ID = item.ID;
                    weekPlan.DatePlan = item.DatePlan;
                    weekPlan.UserID = item.UserID;
                    weekPlan.ContentPlan = item.ContentPlan;
                    weekPlan.Result = item.Result;
                    weekPlan.CreatedDate = item.CreatedDate;
                    weekPlan.CreatedBy = item.CreatedBy;
                    weekPlan.UpdatedDate = DateTime.Now;
                    weekPlan.UpdatedBy = item.UpdatedBy;
                    if (string.IsNullOrWhiteSpace(weekPlan.ContentPlan) && string.IsNullOrWhiteSpace(weekPlan.Result) && weekPlan.ID > 0)
                    {
                        await _weekPlanRepo.DeleteAsync(weekPlan.ID);
                    }
                    if (!string.IsNullOrWhiteSpace(weekPlan.ContentPlan) || !string.IsNullOrWhiteSpace(weekPlan.Result))
                    {
                        if (weekPlan.ID > 0)
                        {
                            await _weekPlanRepo.UpdateAsync(weekPlan);
                        }
                        else
                        {
                            await _weekPlanRepo.CreateAsync(weekPlan);

                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(PlanWeekDTO dto)
        {
            try
            {
                List<WeekPlan> weekPlans = _weekPlanRepo.GetAll().Where(x => x.UserID == dto.userId && x.DatePlan == dto.datePlan).ToList();

                if (!weekPlans.Any())
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy kế hoạch để xoá!"));
                await _weekPlanRepo.DeleteRangeAsync(weekPlans);


                return Ok(ApiResponseFactory.Success(null, "Xoá thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

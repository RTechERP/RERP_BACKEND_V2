using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.VisionBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanWeekController : ControllerBase
    {
        WeekPlanRepo _weekPlanRepo = new WeekPlanRepo();
        DepartmentRepo _departmentRepo = new DepartmentRepo();

        [HttpGet]
        public IActionResult Get(DateTime dateStart, DateTime dateEnd, int departmentId, int userId, int groupSaleId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPlanWeek",
                                        new string[] { "@DateStart", "@DateEnd", "@Department", "@UserID", "@GroupSaleID" },
                                        new object[] { dateStart, dateEnd, departmentId, userId, groupSaleId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                var data1 = SQLHelper<dynamic>.GetListData(list,1);
                return Ok(ApiResponseFactory.Success(new { data, data1}, ""));
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
                    if (weekPlan.ID > 0)
                    {
                        await _weekPlanRepo.UpdateAsync(weekPlan);
                    }
                    else
                    {
                        await _weekPlanRepo.CreateAsync(weekPlan);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

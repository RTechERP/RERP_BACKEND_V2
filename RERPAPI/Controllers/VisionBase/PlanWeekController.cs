using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.VisionBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanWeekController : ControllerBase
    {
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
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
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
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
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
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

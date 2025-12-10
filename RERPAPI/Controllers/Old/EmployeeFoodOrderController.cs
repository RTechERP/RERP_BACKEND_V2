using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeFoodOrderController : ControllerBase
    {
        private readonly EmployeeFoodOrderRepo _employeeFoodOrderRepo;
        public EmployeeFoodOrderController(EmployeeFoodOrderRepo employeeFoodOrderRepo)
        {
            _employeeFoodOrderRepo = employeeFoodOrderRepo;
        }

        [HttpGet("day-of-week")]
        [RequiresPermission("N2,N23,N34,N1,N52,N80")]
        public IActionResult GetDayOfWeek(int month, int year)
        {
            try
            {
                var dt = SQLHelper<object>.ProcedureToList("spGetDayOfWeek", new string[] { "@Month", "@Year" }, new object[] { month, year });
                var result = SQLHelper<object>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost]
        [RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetEmployeeFoodOrder(EmployeeFoodOrderParam param)
        {
            try
            {
                param.dateStart = param.dateStart.Date;
                param.dateEnd = param.dateEnd.Date.AddDays(1).AddSeconds(-1);
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetFoodOrder", 
                    new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID" },
                    new object[] { param.pageNumber, param.pageSize, param.dateStart, param.dateEnd, param.keyWord, param.employeeId });

                var result = SQLHelper<object>.GetListData(foodOrders, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("food-order")]
        [RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetEmployeeFoodOrderByMonth(EmployeeFoodOrderByMonthParam param)
        {
            try
            {
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword", "@Location" },
                                       new object[] { param.month, param.year, param.departmentId, param.employeeId, param.keyword ?? "", param.location });

                var result = SQLHelper<object>.GetListData(foodOrders, 0);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("report-order")]
        [RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetReportFoodOrderByMonth(EmployeeFoodOrderByMonthParam param)
        {
            try
            {
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" },
                                       new object[] { param.month, param.year, param.departmentId, param.employeeId, param.keyword ?? "" });

                var result = SQLHelper<object>.GetListData(foodOrders, 1);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        [RequiresPermission("N2,N23,N34,N1,N80")]
        public async Task<IActionResult> SaveEmployeeFoodOrder([FromBody] EmployeeFoodOrder foodOrder)
        {
            try
            {
                var checkExist = _employeeFoodOrderRepo
                .GetAll(x => x.EmployeeID == foodOrder.EmployeeID
                          && x.DateOrder.Value.Date == foodOrder.DateOrder.Value.Date 
                          && x.IsApproved != true 
                          && x.IsDeleted != true)
                .FirstOrDefault();

                if(checkExist != null)
                {
                    foodOrder.ID = checkExist.ID;
                    foodOrder.Quantity = foodOrder.Quantity + checkExist.Quantity;
                }

                if (foodOrder.ID <= 0) await _employeeFoodOrderRepo.CreateAsync(foodOrder);
                else await _employeeFoodOrderRepo.UpdateAsync(foodOrder);
                return Ok(ApiResponseFactory.Success(foodOrder, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

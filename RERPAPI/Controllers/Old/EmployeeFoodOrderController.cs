using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

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
        public IActionResult GetDayOfWeek(int month, int year)
        {
            try
            {
                var dt = SQLHelper<object>.ProcedureToList("spGetDayOfWeek", new string[] { "@Month", "@Year" }, new object[] { month, year });
                var result = SQLHelper<object>.GetListData(dt, 0);

                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }


        [HttpPost]
        public IActionResult GetEmployeeFoodOrder(EmployeeFoodOrderParam param)
        {
            try
            {
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetFoodOrder", new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID" },
                    new object[] {param.pageNumber , param.pageSize, param.dateStart, param.dateEnd, param.keyWord, param.employeeId });

                var result = SQLHelper<object>.GetListData(foodOrders, 0);

                return Ok(new
                {
                    status = 1,
                    data = result
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {   
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("food-order")]
        public IActionResult GetEmployeeFoodOrderByMonth(EmployeeFoodOrderByMonthParam param)
        {
            try
            {
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" },
                                       new object[] { param.month, param.year, param.departmentId, param.employeeId, param.keyWord ?? ""});

                var result = SQLHelper<object>.GetListData(foodOrders, 0);
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("report-order")]
        public IActionResult GetReportFoodOrderByMonth(EmployeeFoodOrderByMonthParam param)
        {
            try
            {
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" },
                                       new object[] { param.month, param.year, param.departmentId, param.employeeId, param.keyWord ?? "" });

                var result = SQLHelper<object>.GetListData(foodOrders, 1);
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveEmployeeFoodOrder([FromBody] EmployeeFoodOrder foodOrder)
        {
            try
            {
                //var checkOrder = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByDate", new string[] { "@Date", "@EmployeeID", "@ID" }, new object[] { foodOrder.DateOrder, foodOrder.EmployeeID, foodOrder.ID });
                //if (checkOrder != null && checkOrder.Count > 0)
                //{
                //    return BadRequest(new
                //    {
                //        status = 0,
                //        message = "Nhân viên đã đặt cơm ngày " + foodOrder.DateOrder
                //    });
                //}


               if(foodOrder.ID <= 0)
                {
                    await _employeeFoodOrderRepo.CreateAsync(foodOrder);
                } else
                {
                    await _employeeFoodOrderRepo.UpdateAsync(foodOrder);
                }
                return Ok(new
                {
                    status = 1,
                    data = foodOrder,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}

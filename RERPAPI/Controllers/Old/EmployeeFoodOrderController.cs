    using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeFoodOrderController : ControllerBase
    {
        private readonly EmployeeFoodOrderRepo _employeeFoodOrderRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        PhasedAllocationPersonRepo _phaseRepo;
        PhasedAllocationPersonDetailRepo _phaseDetailRepo;

        public EmployeeFoodOrderController(EmployeeFoodOrderRepo employeeFoodOrderRepo, vUserGroupLinksRepo vUserGroupLinksRepo, PhasedAllocationPersonRepo phaseRepo, PhasedAllocationPersonDetailRepo phasedAllocationPersonDetailRepo
)
        {
            _phaseRepo = phaseRepo;
            _employeeFoodOrderRepo = employeeFoodOrderRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _phaseDetailRepo = phasedAllocationPersonDetailRepo;
        }

        [HttpGet("day-of-week")]
        //[RequiresPermission("N2,N23,N34,N1,N52,N80")]
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
        //[RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetEmployeeFoodOrder(EmployeeFoodOrderParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo
      .GetAll()
      .FirstOrDefault(x =>
            (x.Code == "N23" || x.Code == "N1" || x.Code == "N2" || x.Code == "N34") &&
          x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = param.employeeId;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                //var ds = param.dateStart.Date.AddHours(00).AddMinutes(00).AddSeconds(00).AddSeconds(-1);
                //var de = param.dateEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddSeconds(+1);

                var ds = param.dateStart.Date; ;
                var de = param.dateEnd.Date.AddDays(+1).AddSeconds(-1);
                //DateTime dateStart = param.dateStart.Date;
                //DateTime dateEnd = param.dateEnd.Date.AddDays(1).AddTicks(-1);
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetFoodOrder", new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID" },
                    new object[] { param.pageNumber, param.pageSize, ds, de, param.keyWord, employeeID });

                var result = SQLHelper<object>.GetListData(foodOrders, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("food-order")]
        //[RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetEmployeeFoodOrderByMonth(EmployeeFoodOrderByMonthParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo
      .GetAll()
      .FirstOrDefault(x =>
          (x.Code == "N23" || x.Code == "N1" || x.Code == "N2" || x.Code == "N34" || x.Code == "N80") &&
          x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = param.employeeId;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword", "@Location" },
                                       new object[] { param.month, param.year, param.departmentId, employeeID, param.keyword ?? "", param.location });

                var result = SQLHelper<object>.GetListData(foodOrders, 0);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("report-order")]
        //[RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetReportFoodOrderByMonth(EmployeeFoodOrderByMonthParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo
      .GetAll()
      .FirstOrDefault(x =>
          (x.Code == "N23" || x.Code == "N1" || x.Code == "N2" || x.Code == "N34" || x.Code == "N80") &&
          x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = param.employeeId;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var foodOrders = SQLHelper<object>.ProcedureToList("spGetEmployeeFoodOrderByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" },
                                       new object[] { param.month, param.year, param.departmentId, employeeID, param.keyword ?? "" });

                var result = SQLHelper<object>.GetListData(foodOrders, 1);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        //[RequiresPermission("N2,N23,N34,N1,N80")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeFoodOrder foodOrder)
        {
            try
            {
                var now = DateTime.Now;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var today = now.Date;
                var yesterday = today.AddDays(-1);
                // Các quyền được phép thao tác theo EmployeeID tùy ý
                string[] allowCodes = new[] { "N23", "N1", "N2", "N34", "N80" };

                // Kiểm tra user có thuộc nhóm quyền HR hay không
                var vUserHR = _vUserGroupLinksRepo
                    .GetAll()
                    .FirstOrDefault(x => (x.Code == "N23" || x.Code == "N1" || x.Code == "N2" || x.Code == "N34" || x.Code == "N80") && x.UserID == currentUser.ID);
                var today10AM = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0);

                if (vUserHR == null && now > today10AM && currentUser.IsAdmin != true)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,
                        "Bạn chỉ có thể đặt cơm trước 10h sáng hằng ngày."));
                }
                // Gán lại cho foodOrder
                var checkExist = _employeeFoodOrderRepo
                .GetAll(x => x.EmployeeID == foodOrder.EmployeeID
                          && x.DateOrder.Value.Date == foodOrder.DateOrder.Value.Date
                          && foodOrder.IsApproved != true
                          && x.IsDeleted != true)
                .FirstOrDefault();

                if (checkExist != null&& foodOrder.IsDeleted != true)
                {
                    foodOrder.ID = checkExist.ID;
                    foodOrder.Quantity = foodOrder.Quantity + checkExist.Quantity;
                }
                if (foodOrder.IsDeleted == true && vUserHR == null && foodOrder.DateOrder.HasValue)
                {
                    var orderDate = foodOrder.DateOrder.Value.Date;
                    if (orderDate < today)
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            "Không thể xoá phiếu đặt cơm của các ngày trước."
                        ));
                    }
                    if (orderDate == today && now > today10AM)
                    {
                        return BadRequest(ApiResponseFactory.Fail(
                            null,
                            "Sau 10h không thể xoá phiếu đặt cơm của ngày hôm nay."
                        ));
                    }
                }
                if (foodOrder.ID <= 0) await _employeeFoodOrderRepo.CreateAsync(foodOrder);
                else
                {
                    if (foodOrder.ID > 0 && vUserHR == null && foodOrder.DateOrder.HasValue)
                    {
                        var orderDate = foodOrder.DateOrder.Value.Date;
                        if (orderDate < today)
                        {
                            return BadRequest(ApiResponseFactory.Fail(
                                null,
                                "Không thể sửa đặt cơm của ngày hôm trước."
                            ));
                        }
                        if (orderDate == today && now > today10AM && currentUser.IsAdmin != true)
                        {
                            return BadRequest(ApiResponseFactory.Fail(
                                null,
                                "Không thể sửa đặt cơm sau 10H ngày hôm nay."
                            ));
                        }
                    }

                    await _employeeFoodOrderRepo.UpdateAsync(foodOrder);
                }
                return Ok(ApiResponseFactory.Success(foodOrder, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N2,N23,N34,N1")]
        [HttpPost("save-approve")]
        public async Task<IActionResult> SaveApprove([FromBody] List<EmployeeFoodOrder> foodOrders)
        {
            try
            {
                foreach (var item in foodOrders)
                {
                    if (item.ID <= 0) await _employeeFoodOrderRepo.CreateAsync(item);
                    else
                    {
                        await _employeeFoodOrderRepo.UpdateAsync(item);
                    }
                }
                //Lưu phase cấp phát
                await _phaseRepo.UpdatePhaseFromFoodOrder(foodOrders);
                return Ok(ApiResponseFactory.Success(foodOrders, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

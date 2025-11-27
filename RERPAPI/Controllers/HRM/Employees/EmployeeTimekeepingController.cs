using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EmployeeTimekeepingController : ControllerBase
    {
        private readonly EmployeeChamCongMasterRepo _employeeChamCongMaster;
        private readonly EmployeeChamCongDetailRepo _employeeChamCongDetail;
        private readonly DepartmentRepo _departmentRepo;
        private readonly EmployeeRepo _employeeRepo;

        public EmployeeTimekeepingController(
            EmployeeChamCongMasterRepo employeeChamCongMaster,
            EmployeeChamCongDetailRepo employeeChamCongDetail,
            DepartmentRepo departmentRepo,
            EmployeeRepo employeeRepo)
        {
            _employeeChamCongMaster = employeeChamCongMaster;
            _employeeChamCongDetail = employeeChamCongDetail;
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-employee-timekeeping")]
        public IActionResult GetEmployeeTimekeeping(int year, string? keyword)
        {

            try
            {
                var dt = SQLHelper<object>.ProcedureToList("spGetEmployeeChamCongMaster",
                    new string[] { "@Year", "@Keyword" },
                    new object[] { year, keyword ?? "" });

              
                return Ok(ApiResponseFactory.Success(dt, "Lấy dữ lệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-employee-timekeeping/{id}")]
        public IActionResult GetEmployeeTimekeepingID(int id)
        {
            try
            {
                var employeeTimekeeping = _employeeChamCongMaster.GetByID(id);
                if (employeeTimekeeping == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin chấm công"));
                }
                return Ok(ApiResponseFactory.Success(employeeTimekeeping, "Lấy chi tiết chấm công thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }


        }
        [RequiresPermission("N1,N2")]

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeChamCongMaster employeeTimekeeping)
        {
            try
            {

                if (employeeTimekeeping.ID <= 0) await _employeeChamCongMaster.CreateAsync(employeeTimekeeping);
                else await _employeeChamCongMaster.UpdateAsync(employeeTimekeeping);
                return Ok(ApiResponseFactory.Success(employeeTimekeeping, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("check-duplicate-employeetimekeeping/{id}/{month}/{year}")]
        public IActionResult CheckDuplicateEmployeeTimekeeping(int id, int month, int year)
        {
            try
            {
                bool isDuplicate = _employeeChamCongMaster.GetAll()
                    .Any(x => x.ID != id &&
                              x._Month == month &&
                              x._Year == year
                              && x.IsDeleted == false
                              );

                return Ok(new
                {
                    status = 1,
                    isDuplicate // Trả về đúng tên trường FE đang dùng
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var departments = _departmentRepo.GetAll()
                    .Select(x => new { x.ID, x.Code, x.Name })
                    .ToList();

                return Ok(new { status = 1, data = departments });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
                var employees = _employeeRepo.GetAll()
                    .Select(x => new { x.ID, x.Code, x.FullName })
                    .ToList();

                return Ok(new { status = 1, data = employees });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-timekeeping-data")]
        public IActionResult GetTimekeepingData(
            int employeeId = 0,
            int month = 0,
            int year = 0,
            int departmentId = 0,
            string? keyword = ""
        )
        {
            try
            {
                // GỌI PROC: Lưu ý tên tham số đúng với proc
                var ds = SQLHelper<object>.ProcedureToList(
                    "spGetChamCongNew",
                    new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@KeyWord" },
                    new object[] { month, year, departmentId, employeeId, keyword ?? "" }
                );

                // Helper an toàn lấy table theo index
                List<object> Table(int idx)
                {
                    try { return SQLHelper<object>.GetListData(ds, idx) ?? new List<object>(); }
                    catch { return new List<object>(); }
                }

                // 0) Bảng chính
                var data = Table(0);

                // 1) Lịch ngày trong tháng (pivot D1..D31) 
                var weekdays = Table(1).FirstOrDefault();

                // 2) Tổng ngày công chuẩn (1 dòng 1 cột: TotalWorkday)
                int totalWorkday = 0;
                var ws = Table(2).FirstOrDefault();
                if (ws != null)
                {
                    // đọc động thuộc tính "TotalWorkday"
                    var dict = ws as IDictionary<string, object>;
                    if (dict != null && dict.TryGetValue("TotalWorkday", out var v) && v != null)
                    {
                        int.TryParse(v.ToString(), out totalWorkday);
                    }
                    else
                    {
                        // fallback cho trường hợp kiểu dynamic khác
                        var prop = ws.GetType().GetProperty("TotalWorkday");
                        if (prop != null && prop.GetValue(ws) != null)
                            int.TryParse(prop.GetValue(ws)!.ToString(), out totalWorkday);
                    }
                }

                // 3) Tổng holiday theo nhân viên
                var holidays = Table(3); // mỗi item có EmployeeID, TotalHoliday

                // 4) Chi tiết holiday áp theo ngày
                var holidayDetails = Table(4);

                return Ok(new
                {
                    status = 1,
                    data,             // bảng chính (nhân viên + D1..D31 + totals)
                    weekdays,         // pivot D1..D31: "Tn;status"
                    totalWorkday,     // số ngày công chuẩn của tháng (đã trừ CN + T7 off)
                    holidays,         // tổng ngày lễ theo Employee
                    holidayDetails    // chi tiết các ngày lễ theo từng dòng chấm công
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-timekeeping-detail-data")]
        public IActionResult GetTimekeepingDetailData(
            int employeeId = 0,
            int month = 0,
            int year = 0,
            int departmentId = 0,
            string? keyword = ""
        )
        {
            try
            {
                var ds = SQLHelper<object>.ProcedureToList(
                    "spGetEmployeeChamCongDetail",
                    new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@KeyWord" },
                    new object[] { month, year, departmentId, employeeId, keyword ?? "" }
                );


                return Ok(new
                {
                    status = 1,
                    data = ds

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("update-all")]
        public IActionResult UpdateAll(
            [FromQuery] int masterId,
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] string loginName = ""
        )
        {
            try
            {
                var existDetails = _employeeChamCongDetail.GetAll(x => x.MasterID == masterId && x.IsDeleted == false);
                foreach (var detail in existDetails)
                {
                    detail.IsDeleted = true;
                    _employeeChamCongDetail.Update(detail);
                }
                SQLHelper<object>.ExcuteProcedure(
                    "spInsertIntoEmployeeChamCongDetail",
                    new[] { "@MasterID", "@Month", "@Year", "@EmployeeID", "@LoginName" },
                    new object[] { masterId, month, year, 0, loginName ?? "" }
                );


                return Ok(ApiResponseFactory.Success(null, "Cập nhật toàn bộ bảng công thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("update-one")]
        public IActionResult UpdateOne(
            [FromQuery] int masterId,
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] int employeeId,
            [FromQuery] string loginName = ""
        )
        {
            try
            {
                if (employeeId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy nhân viên"));

                // XÓA chi tiết của 1 nhân viên trong master
                //SQLHelper<object>.ExecuteNonQuery(
                //    "DELETE FROM dbo.EmployeeChamCongDetail WHERE MasterID = @MasterID AND EmployeeID = @EmployeeID",
                //    new[] { "@MasterID", "@EmployeeID" },
                //    new object[] { masterId, employeeId }
                //);
                var existDetails = _employeeChamCongDetail.GetAll(x => x.MasterID == masterId && x.EmployeeID == employeeId && x.IsDeleted == false);
                foreach (var detail in existDetails)    
                {
                    detail.IsDeleted = true;
                    _employeeChamCongDetail.Update(detail);
                }
                // CHÈN LẠI cho nhân viên đó
                SQLHelper<object>.ExcuteProcedure(
                    "spInsertIntoEmployeeChamCongDetail",
                    new[] { "@MasterID", "@Month", "@Year", "@EmployeeID", "@LoginName" },
                    new object[] { masterId, month, year, employeeId, loginName ?? "" }
                );


                return Ok(ApiResponseFactory.Success(null, "Cập nhật công nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
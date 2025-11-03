using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.Data;
namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeWFHController : ControllerBase
    {
        private readonly EmployeeWFHRepo _employeeWFHRepo = new EmployeeWFHRepo();



        [HttpGet("getwfh")]
        public IActionResult GetWFH(
          [FromQuery] int page,
          [FromQuery] int size,
          [FromQuery] int year,
          [FromQuery] int month,
          [FromQuery] string keyword = "",
          [FromQuery] int departmentId = 0,
          [FromQuery] int idApprovedTP = 0,
          [FromQuery] int status = -1)
        {
            try
            {
                // Gọi stored procedure
                var dt = SQLHelper<EmployeeWFHDTO>.ProcedureToList("spGetWFH",
                    new string[] { "@PageNumber", "@PageSize", "@Year", "@Month", "@Keyword", "@DepartmentID", "@IDApprovedTP", "@Status" },
                    new object[] { page, size, year, month, keyword, departmentId, idApprovedTP, status });

                // Dữ liệu trang hiện tại
                var data = SQLHelper<EmployeeWFHDTO>.GetListData(dt, 0);

                // Tổng số trang
                int totalPage = 0;
                var totalPageTable = SQLHelper<object>.GetListData(dt, 1);
                if (totalPageTable.Count > 0)
                {
                    dynamic row = totalPageTable[0];
                    totalPage = row.TotalPage;
                }

                // Trả kết quả JSON
                return Ok(new
                {
                    status = 1,
                    message = "Lấy danh sách WFH thành công",

                    data = data,
                    totalPage = totalPage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Lỗi khi lấy danh sách WFH",
                    error = ex.Message
                });
            }
        }


        [HttpGet("wfh-detail/{id}")]
        public IActionResult GetWFHDetail(int id)
        {
            try
            {
                var wfhDetail = _employeeWFHRepo.GetByID(id);
                if (wfhDetail == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin WFH"));
                }

                return Ok(ApiResponseFactory.Success(wfhDetail, "Lấy chi tiết WFH thành công"));
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
                var departmentRepo = new DepartmentRepo();

                var departments = departmentRepo.GetAll()
                    .Select(x => new
                    {
                        x.ID,
                        x.Code,
                        x.Name
                    }).ToList();

                return Ok(new
                {
                    status = 1,
                    data = departments
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeWFH employeeWFH)
        {
            try
            {

                if (employeeWFH.ID <= 0) await _employeeWFHRepo.CreateAsync(employeeWFH);
                else await _employeeWFHRepo.UpdateAsync(employeeWFH);
                return Ok(ApiResponseFactory.Success(employeeWFH, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-approver")]
        public IActionResult GetEmployeesWithApprovers()
        {
            try
            {
                // Gọi store procedure
                var dataSet = SQLHelper<Object>.ProcedureToList("spGetEmployeeAndEmployeeApprover", new string[] { }, new object[] { });

                var employees = SQLHelper<object>.GetListData(dataSet, 0)
                                     .Select(x => new
                                     {
                                         x.ID,
                                         x.FullName,
                                         x.DepartmentName,
                                         x.Code

                                     }).ToList();

                var approvers = SQLHelper<object>.GetListData(dataSet, 1)
                                                         .Select(x => new
                                                         {
                                                             x.EmployeeID,
                                                             x.FullName,
                                                             x.DepartmentName,
                                                             x.Code

                                                         }).ToList();

                // Trả về kết quả
                return Ok(new
                {
                    status = 1,
                    message = "Lấy danh sách nhân viên và người phê duyệt thành công",
                    data = new
                    {
                        employees,
                        approvers
                    },
                });
            }
            catch (Exception ex)

            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-duplicate-wfh/{id}/{employeeId}/{date}/{timeWFH}")]
        public IActionResult CheckDuplicateWFH(int id, int employeeId, string date, int timeWFH)
        {
            try
            {
                bool isDuplicate = false;

                // Chuyển đổi chuỗi ngày sang DateTime
                DateTime dateWFH;
                if (!DateTime.TryParse(date, out dateWFH))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Định dạng ngày không hợp lệ!"
                    });
                }

                var existWFH = _employeeWFHRepo.GetAll()
                    .Where(x => x.ID != id &&
                                x.EmployeeID == employeeId &&
                                x.DateWFH.HasValue &&
                                x.DateWFH.Value.Date == dateWFH.Date &&
                                x.TimeWFH == timeWFH
                                //&& x.IsDelete == false

                                );

                if (existWFH.Any())
                {
                    isDuplicate = true;
                }

                return Ok(new
                {
                    status = 1,
                    data = isDuplicate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = "Lỗi kiểm tra trùng WFH",
                    error = ex.Message
                });
            }
        }


    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity;
using System.Data;


namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class EmployeeNoFingerprintController : ControllerBase
    {
        EmployeeNoFingerprintRepo _employeeNoFingerprintRepo;
        DepartmentRepo _departmentRepo;

        public EmployeeNoFingerprintController(EmployeeNoFingerprintRepo employeeNoFingerprintRepo, DepartmentRepo departmentRepo)
        {
            _employeeNoFingerprintRepo = employeeNoFingerprintRepo;
            _departmentRepo = departmentRepo;
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("get-employee-no-fingerprint")]
        public IActionResult GetEmployeeNoFingerprint([FromBody] EmployeeNoFingerPrintRequestParam request)
        {
            try
            {
                DateTime ds = new DateTime(request.DateStart.Year, request.DateStart.Month, request.DateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(request.DateEnd.Year, request.DateEnd.Month, request.DateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spGetEmployeeNoFingerprint",
                                                   new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@DepartmentID", "@IDApprovedTP", "@Status", @"Keyword" },
                                                   new object[] { request.Page ?? 1, request.Size ?? 50, ds, de, request.DepartmentID ?? 0, request.IDApprovedTP, request.Status, request.KeyWord ?? "" });
                var data = SQLHelper<object>.GetListData(dt, 0);
                var totalPage = SQLHelper<object>.GetListData(dt, 1);

                return Ok(ApiResponseFactory.Success(new { data, totalPage }, "Lấy dữ lệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpGet("get-employee-approver")]
        public IActionResult GetEmployeesWithApprovers()
        {
            try
            {
                // Gọi store procedure
                var dataSet = SQLHelper<object>.ProcedureToList("spGetEmployeeAndEmployeeApprover", new string[] { }, new object[] { });

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
        [RequiresPermission("N1,N2")]
        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeNoFingerprint employeeNoFingerprint)
        {
            try
            {

                if (employeeNoFingerprint.ID <= 0)
                    await _employeeNoFingerprintRepo.CreateAsync(employeeNoFingerprint);
                else
                    await _employeeNoFingerprintRepo.UpdateAsync(employeeNoFingerprint);
                return Ok(ApiResponseFactory.Success(employeeNoFingerprint, "Cập nhật thành công!"));
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
        [RequiresPermission("N1,N2")]

        [HttpGet("check-duplicate-enf/{id}/{employeeId}/{dayWork}/{type}")]
        public IActionResult CheckDuplicateENF(int id, int employeeId, string dayWork, int type)
        {
            try
            {
                bool isDuplicate = false;

                // Chuyển đổi chuỗi ngày sang DateTime
                DateTime dayWorkDate;
                if (!DateTime.TryParse(dayWork, out dayWorkDate))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Định dạng ngày không hợp lệ!"
                    });
                }

                var existENF = _employeeNoFingerprintRepo.GetAll(x => x.ID != id &&
                                x.EmployeeID == employeeId &&
                                x.DayWork.HasValue &&
                                x.DayWork.Value.Date == dayWorkDate.Date &&
                                x.Type == type
                                //&& x.IsDelete == false
                                );

                if (existENF.Any())
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
                    message = "Lỗi kiểm tra trùng ENF",
                    error = ex.Message
                });
            }
        }


    }

}
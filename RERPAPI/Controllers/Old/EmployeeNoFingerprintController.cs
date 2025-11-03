using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;


namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeNoFingerprintController : ControllerBase
    {
        private readonly EmployeeNoFingerprintRepo _employeeNoFingerprintRepo = new EmployeeNoFingerprintRepo();

        [HttpGet("get-employee-no-fingerprint")]
        public async Task<IActionResult> getEmployeeNoFingerprint(int pageNumber, int pageSize, DateTime dateStart, DateTime dateEnd, int departmentId, int idApprovedTP, int status, string? keyword)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spGetEmployeeNoFingerprint",
                                                   new string[] { "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@DepartmentID", "@IDApprovedTP", "@Status", @"Keyword" },
                                                   new object[] { pageNumber, pageSize, ds, de, departmentId, idApprovedTP, status, keyword ?? "" });
                // Lấy từng bảng trong DataSet
                var data = SQLHelper<object>.GetListData(dt, 0);
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

                    data,
                    totalPage
                });
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
                return Ok(ApiResponseFactory.Success(new
                {
                    employees,
                    approvers
                }, "Cập nhật thành công!"));
            }
            catch (Exception ex)

            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeNoFingerprint employeeNoFingerprint)
        {
            try
            {

                if (employeeNoFingerprint.ID <= 0) await _employeeNoFingerprintRepo.CreateAsync(employeeNoFingerprint);
                else await _employeeNoFingerprintRepo.UpdateAsync(employeeNoFingerprint);
                return Ok(ApiResponseFactory.Success(employeeNoFingerprint, "Cập nhật thành công!"));
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

                return Ok(ApiResponseFactory.Success(departments, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


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
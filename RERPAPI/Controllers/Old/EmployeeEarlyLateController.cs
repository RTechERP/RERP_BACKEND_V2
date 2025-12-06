using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.ComponentModel;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeEarlyLateController : Controller
    {
        private readonly EmployeeEarlyLateRepo _employeeEarlyLateRepo;
        public EmployeeEarlyLateController(EmployeeEarlyLateRepo employeeEarlyLateRepo)
        {
            _employeeEarlyLateRepo = employeeEarlyLateRepo;
        }
        [HttpPost]
        [RequiresPermission("N2,N1")]
        public IActionResult GetEmployeeEarlyLate(EmployeeEarlyLateParam param)
        {
            try
            {
                var arrParamName = new string[] { "@FilterText", "@PageNumber", "@PageSize", "@Month", "@Year", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.keyWord ?? "", param.pageNumber, param.pageSize, param.month, param.year, param.departmentId, param.idApprovedTp, param.status};
                var employeeEarlyLate = SQLHelper<object>.ProcedureToList("spGetEmployeeEarlyLate", arrParamName, arrParamValue);
                return Ok(new {
                    data = SQLHelper<object>.GetListData(employeeEarlyLate, 0),
                    status = 1
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
        [HttpPost("get-employee-early-late-person")]

        public IActionResult GetEmployeeEarlyLatePerson(EmployeeOnLeavePersonParam request)
        {
            try
            {
                var employeelate = SQLHelper<object>.ProcedureToList("spGetEmployeeEarlyLate_New", new string[] { "@PageNumber", "@PageSize", "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DepartmentID" },
               new object[] { request.Page, request.Size, request.Keyword ?? "", request.DateStart, request.DateEnd, request.IDApprovedTP, request.Status, request.DepartmentID });

                var data = SQLHelper<object>.GetListData(employeelate, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeelate, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> SaveEmployeeEarlyLate([FromBody] EmployeeEarlyLate employeeEarlyLate)
        {
            try
            {

               if(employeeEarlyLate.ID <= 0)
                {
                    if (employeeEarlyLate.DateStart.HasValue)
                    {
                        employeeEarlyLate.DateStart = DateTime.SpecifyKind(employeeEarlyLate.DateStart.Value, DateTimeKind.Utc);
                    }
                    if (employeeEarlyLate.DateEnd.HasValue)
                    {
                        employeeEarlyLate.DateEnd = DateTime.SpecifyKind(employeeEarlyLate.DateEnd.Value, DateTimeKind.Utc);
                    }
                    var exisingEmployeeEarlyLate = _employeeEarlyLateRepo.GetAll().Where(x =>  x.EmployeeID == employeeEarlyLate.EmployeeID && 
                                                                                                 x.DateRegister.Value.Date == employeeEarlyLate.DateRegister.Value.Date &&
                                                                                                    x.Type == employeeEarlyLate.Type &&
                                                                                                     x.ID != employeeEarlyLate.ID);

                    if (exisingEmployeeEarlyLate.Any())
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Nhân viên đã khai báo ngày này rồi! Vui lòng kiếm tra lại",
                        });
                    }
                        var result = await _employeeEarlyLateRepo.CreateAsync(employeeEarlyLate);
                        return Ok(new
                        {
                            status = 1,
                            message = "Thêm thành công",
                            data = result
                        });
                    }
                    else
                    {
                        var result = await _employeeEarlyLateRepo.UpdateAsync(employeeEarlyLate);
                        return Ok(new
                        {
                            status = 1,
                            message = "Cập nhật thành công",
                            data = result
                        });
                    }
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
    
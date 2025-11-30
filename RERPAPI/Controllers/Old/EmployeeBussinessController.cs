using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeBussinessController : ControllerBase
    {
        private EmployeeBussinessRepo _employeeBussinessRepo;
        public EmployeeBussinessController(EmployeeBussinessRepo employeeBussinessRepo)
        {
            _employeeBussinessRepo = employeeBussinessRepo;
        }
        [HttpPost]
        public IActionResult getEmployeeBussiness(EmployeeBussinessParam param)
        {
            try
            {
                var arrParamName = new string[] { "@PageNumber", "@PageSize", "@StartDate", "@EndDate", "@Keyword", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.pageNumber, param.pageSize, param.dateStart, param.dateEnd, param.keyWord, param.departmentId, param.idApprovedTp, param.status };
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussiness", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);

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

        [HttpGet("detail")]
        public async Task<IActionResult> getEmployeeBussinessDetail(int employeeId, DateTime dayBussiness)
        {
            try
            {
                var arrParamName = new string[] { "@EmployeeID", "@DayBussiness" };
                var arrParamValue = new object[] { employeeId, dayBussiness };
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussinessDetail", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> deleteEmployeeBussiness([FromQuery] List<int> listID)
        {
            try
            {
                if (listID.Count() > 0)
                {
                    foreach (var id in listID)
                    {
                        var employeeBussiness = _employeeBussinessRepo.GetByID(id);
                        if (employeeBussiness != null) await _employeeBussinessRepo.DeleteAsync(id);

                    }
                }
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }




        [HttpPost("save-data")]
        public async Task<IActionResult> saveEmployeeBussiness([FromBody] List<EmployeeBussiness> employeeBussiness)
        {
            try
            {
                if (employeeBussiness.Count() > 0)
                {
                    foreach (EmployeeBussiness item in employeeBussiness)
                    {
                        if (item.ID <= 0)
                        {
                            item.IsApproved = false;
                            item.IsApprovedBGD = false;
                            item.IsApprovedHR = false;
                            await _employeeBussinessRepo.CreateAsync(item);
                        }
                        else await _employeeBussinessRepo.UpdateAsync(item);
                    }
                }


                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

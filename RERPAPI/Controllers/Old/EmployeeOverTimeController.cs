using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOverTimeController : ControllerBase
    {
        private readonly EmployeeOverTimeRepo _employeeOverTimeRepo;
        private readonly EmployeeTypeOverTimeRepo _employeeTypeOvertimeRepo;
        public EmployeeOverTimeController(EmployeeOverTimeRepo employeeOverTimeRepo, EmployeeTypeOverTimeRepo employeeTypeOvertimeRepo)
        {
            _employeeOverTimeRepo = employeeOverTimeRepo;
            _employeeTypeOvertimeRepo = employeeTypeOvertimeRepo;
        }

        [HttpPost]
        [RequiresPermission("N2,N1")]
        public IActionResult GetEmployeeOverTime([FromBody] EmployeeOverTimeParam param)
        {
            try
            {
                var dateStart = param.dateStart.Date; // 00:00:00
                var dateEnd = param.dateEnd.Date.AddDays(1).AddSeconds(-1);
                var arrParamName = new string[] { "@FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.keyWord ?? "", param.pageNumber, param.pageSize, dateStart, dateEnd, param.departmentId, param.idApprovedTp, param.status };
                var employeeOverTime = SQLHelper<object>.ProcedureToList("spGetEmployeeOvertime", arrParamName, arrParamValue);
                return Ok(new
                {
                    data = SQLHelper<object>.GetListData(employeeOverTime, 0),
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

        [HttpPost("save-data")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> SaveEmployeeOverTime([FromBody] EmployeeOverTimeDTO request)
        {
            try
            {
                foreach (var employeeOvertime in request.EmployeeOvertimes ?? new List<EmployeeOvertime>())
                {
                    EmployeeOvertime existingOvertime = null;
                    if (employeeOvertime.ID > 0)
                    {
                        existingOvertime = _employeeOverTimeRepo.GetByID(employeeOvertime.ID);
                    }

                    var employeeOverTime = existingOvertime ?? new EmployeeOvertime();

                    employeeOverTime.EmployeeID = employeeOvertime.EmployeeID;
                    employeeOverTime.ApprovedID = employeeOvertime.ApprovedID;
                    employeeOverTime.DateRegister = employeeOvertime.DateRegister;
                    employeeOverTime.TimeStart = employeeOvertime.TimeStart;
                    employeeOverTime.EndTime = employeeOvertime.EndTime;
                    employeeOverTime.Location = employeeOvertime.Location;
                    employeeOverTime.Overnight = employeeOvertime.Overnight;
                    employeeOverTime.TypeID = employeeOvertime.TypeID;
                    employeeOverTime.Reason = employeeOvertime.Reason;
                    employeeOverTime.ReasonHREdit = employeeOvertime.ReasonHREdit;
                    employeeOverTime.IsApproved = employeeOvertime.IsApproved;
                    employeeOverTime.IsApprovedHR = employeeOvertime.IsApprovedHR;
                    employeeOverTime.IsApprovedBGD = employeeOvertime?.IsApprovedBGD;
                    employeeOverTime.IsDeleted = employeeOvertime.IsDeleted;
                    employeeOverTime.IsProblem = false;
                    //employeeOverTime.IsDeleted = employeeOvertime.IsDeleted;



                    // Calculate TimeReality
                    if (employeeOverTime.TimeStart.HasValue && employeeOverTime.EndTime.HasValue)
                    {
                        employeeOverTime.TimeReality = (decimal)(employeeOverTime.EndTime.Value - employeeOverTime.TimeStart.Value).TotalHours;
                    }

                    if (employeeOverTime.Overnight == true)
                    {
                        employeeOverTime.CostOvernight = 30000;
                    }
                    else
                    {
                        employeeOverTime.CostOvernight = 0;
                    }

                    // Get ratio from EmployeeTypeOvertime
                    var type = _employeeTypeOvertimeRepo.GetByID(Convert.ToInt32(employeeOverTime.TypeID));
                    if (type != null)
                    {
                        employeeOverTime.TotalTime = employeeOverTime.TimeReality * (type.Ratio / 100);
                    }

                    if (employeeOverTime.ID > 0)
                    {

                        await _employeeOverTimeRepo.UpdateAsync(employeeOverTime);
                    }
                    else
                    {
                        employeeOverTime.DecilineApprove = 1;
                        employeeOverTime.CreatedDate = DateTime.Now;
                        employeeOverTime.IsApproved = false;
                        employeeOverTime.IsApprovedHR = false;
                        await _employeeOverTimeRepo.CreateAsync(employeeOverTime);
                    }
                }

                // Delete records if listId is provided
                //if (request.ListId?.Count > 0)
                //{
                //    await _employeeOverTimeRepo.DeleteByIdsAsync(request.ListId);
                //}


                return Ok(new
                {
                    status = 1,
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

        [HttpGet("detail")]
        [RequiresPermission("N2,N1")]
        public IActionResult GetEmployeeOverTimeDetail(int employeeId, DateTime dateRegister)
        {
            try
            {
                var employeeOverTime = _employeeOverTimeRepo
                .GetAll()
                .Where(e => e.EmployeeID == employeeId && e.DateRegister.Value.Date == dateRegister.Date)
                .OrderBy(x => x.TimeStart);
                if (employeeOverTime == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Không tìm thấy thông tin"
                    });
                }

                return Ok(new
                {
                    data = employeeOverTime,
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

        [HttpPost("summary")]
        [RequiresPermission("N2,N1")]
        public IActionResult GetEmployeeOverTimeByMonth(EmployeeOverTimeByMonthParam param)
        {
            try
            {
                var overTimes = SQLHelper<object>.ProcedureToList("spGetEmployeeOvertimeByMonth", new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" },
                                       new object[] { param.month, param.year, param.departmentId, param.employeeId, param.keyWord ?? "" });

                var result = SQLHelper<object>.GetListData(overTimes, 0);
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

    }
}

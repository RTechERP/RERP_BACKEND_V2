﻿using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOnLeaveController : ControllerBase
    {
        EmployeeOnLeaveRepo employeeOnLeaveRepo = new EmployeeOnLeaveRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();

        [HttpPost]
        public IActionResult GetAllEmployeeOnLeave(EmployeeOnLeaveParam param)
        {
            try
            {
                var employeeOnLeaves = SQLHelper<object>.ProcedureToList("spGetDayOff", new string[] { "@PageNumber", "@PageSize", "@Keyword", "@Month", "@Year", "@IDApprovedTP", "@Status", "@DepartmentID" },
                    new object[] { param.pageNumber, param.pageSize, param.keyWord, param.month, param.year, param.IDApprovedTP, param.status, param.departmentId });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employeeOnLeaves, 0)
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

        [HttpGet]
        public IActionResult GetSummaryEmployeeOnLeave(int month, int year, string? keyWord)
        {
            try
            {
                var summary = SQLHelper<object>.ProcedureToList("spGetEmployeeOnleaveByMonth", new string[] { "@Month", "@Year", "@KeyWord" },
                                       new object[] { month, year, keyWord ?? "" });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(summary, 0)
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
        public async Task<IActionResult> SaveEmployeeOnLeave(EmployeeOnLeave employeeOnLeave)
        {
            try
            {
                if (employeeOnLeave.StartDate.HasValue)
                {
                    employeeOnLeave.StartDate = DateTime.SpecifyKind(employeeOnLeave.StartDate.Value, DateTimeKind.Utc);
                }
                if (employeeOnLeave.EndDate.HasValue)
                {
                    employeeOnLeave.EndDate = DateTime.SpecifyKind(employeeOnLeave.EndDate.Value, DateTimeKind.Utc);
                }

                var existingLeaves = employeeOnLeaveRepo.GetAll()
                    .Where(x => x.EmployeeID == employeeOnLeave.EmployeeID
                        && x.TypeIsReal == employeeOnLeave.TypeIsReal
                        && x.ID != employeeOnLeave.ID
                        && x.StartDate.HasValue
                        && employeeOnLeave.StartDate.HasValue
                        && x.StartDate.Value.Date == employeeOnLeave.StartDate.Value.Date);

                if (existingLeaves.Any())
                        {
                            return BadRequest(new
                            {
                                status = 0,
                                message = "Nhân viên đã đăng ký nghỉ ngày " + employeeOnLeave.StartDate.Value.ToString("dd/MM/yyyy") + ". Vui lòng chọn ngày khác.",
                            });
                        }

                if (employeeOnLeave.ID <= 0)
                {
                    await employeeOnLeaveRepo.CreateAsync(employeeOnLeave);
                } else
                {
                    employeeOnLeaveRepo.UpdateFieldsByID(employeeOnLeave.ID, employeeOnLeave);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
                }); ;
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

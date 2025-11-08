using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeScheduleWorkController : ControllerBase
    {
        private readonly EmployeeScheduleWorkRepo _employeeScheduleWorkRepo;

        public EmployeeScheduleWorkController(EmployeeScheduleWorkRepo employeeScheduleWorkRepo)
        {
            _employeeScheduleWorkRepo = employeeScheduleWorkRepo;
        }       

        [HttpGet("schedule-work")]
        public IActionResult GetEmployeeScheduleWork(int month, int year)
        {
            try
            {
                var dtScheduleWork = SQLHelper<object>.ProcedureToList("spGetEmployeeScheduleWorkByDate",
                                                                               new string[] { "@Year", "@Month" },
                                                                                                                                          new object[] { year, month });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(dtScheduleWork, 0)
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

        [HttpGet("register-work")]
        public IActionResult GetEmployeeRegisterWork(int month, int year, int departmentId, string? filterText)
        {
            try
            {
                var dtRegisterWork = SQLHelper<object>.ProcedureToList("spGetEmployeeRegisterWork",
                                                                               new string[] { "@Year", "@Month", "@DepartmentID", "@FilterText" },
                                                                                                                                          new object[] { year, month, departmentId, filterText ?? ""});
                return Ok(new
                {
                    status = 1,
                    //data = SQLHelper<object>.GetListData(dtRegisterWork, 0)
                    data = dtRegisterWork
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
        public async Task<IActionResult> SaveEmployeeScheduleWork([FromBody] EmployeeScheduleWork employeeScheduleWork)
        {
            try
            {
                //if(employeeScheduleWork.IsApproved)
                //{
                //    return BadRequest(new
                //    {
                //        status = 0,
                //        message = "Ngày làm việc ngày đã được duyệt"
                //    });
                //}

                if(employeeScheduleWork.ID <= 0)
                {
                    await _employeeScheduleWorkRepo.CreateAsync(employeeScheduleWork);
                } else
                {
                    await _employeeScheduleWorkRepo.UpdateAsync(employeeScheduleWork);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu lịch làm việc thành công",
                    data = employeeScheduleWork
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

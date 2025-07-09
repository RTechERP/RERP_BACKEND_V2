using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static RERPAPI.Controllers.EmployeeController;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOnLeaveMasterController : ControllerBase
    {
        EmployeeOnLeaveMasterRepo employeeOnLeaveMasterRepo = new EmployeeOnLeaveMasterRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();

        [HttpGet]
        public IActionResult GetAllEmployeeOnLeaveMaster()
        {
            try
            {
                var employeeOnLeaveMasters = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveMaster", new string[] { },
                                       new object[] { });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employeeOnLeaveMasters, 0)
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
        public async Task<IActionResult> SaveEmployeeOnLeaveMaster([FromBody]EmployeeOnLeaveMaster employeeOnLeaveMaster)
        {
            try
            {

                if(employeeOnLeaveMaster.ID <= 0)
                {
                    var employee = SQLHelper<object>.ProcedureToList("spGetCheckDeclareDayOff", new string[] { "@EmployeeID", "ID", "@Year" },
                                                              new object[] { employeeOnLeaveMaster.EmployeeID, employeeOnLeaveMaster.ID, employeeOnLeaveMaster.YearOnleave });
                    var result = SQLHelper<object>.GetListData(employee, 0);

                    if(result.Count > 0)
                    {
                           return BadRequest(new
                           {
                            status = 0,
                            message = "Nhân viên này đã tồn tại trong năm " + employeeOnLeaveMaster.YearOnleave + ", vui lòng kiểm tra lại",
                        });
                    }
                    await employeeOnLeaveMasterRepo.CreateAsync(employeeOnLeaveMaster);
                } else
                {
                    employeeOnLeaveMasterRepo.UpdateFieldsByID(employeeOnLeaveMaster.ID, employeeOnLeaveMaster);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
                }); ;
            } catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }


        public class EmployeeOnLeaveMasterCheck
        {
            public int? EmployeeID { get; set; }
            public decimal? YearOnleave { get; set; }
        }



        [HttpPost("check-exist")]
        public async Task<IActionResult> CheckExist([FromBody] List<EmployeeOnLeaveMasterCheck> check)
        {
            try
            {
                var employeeIDList = check.Select(x => x.EmployeeID).ToList();
                var yearOnLeaveList = check.Select(x => x.YearOnleave).ToList();


                // Kiểm tra trong database
                var existingDayOff = employeeOnLeaveMasterRepo.GetAll()
                    .Where((x => employeeIDList.Contains(x.EmployeeID) && yearOnLeaveList.Contains(x.YearOnleave)))
                    .Select(x => new
                    {
                        x.ID,
                        x.EmployeeID,
                        x.YearOnleave
                    })
                    .ToList();

                return Ok(new
                {
                    data = new
                    {
                        existingDayOff
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

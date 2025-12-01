using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static RERPAPI.Controllers.HRM.EmployeeController;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOnLeaveMasterController : ControllerBase
    {
        private readonly EmployeeOnLeaveMasterRepo _employeeOnLeaveMasterRepo;
        private readonly EmployeeRepo _employeeRepo;
        public EmployeeOnLeaveMasterController(EmployeeOnLeaveMasterRepo employeeOnLeaveMasterRepo, EmployeeRepo employeeRepo)
        {
            _employeeOnLeaveMasterRepo = employeeOnLeaveMasterRepo;
            _employeeRepo = employeeRepo;
        }

        [HttpGet]
        [RequiresPermission("N2,N1")]
        public IActionResult GetAllEmployeeOnLeaveMaster()
        {
            try
            {
                var employeeOnLeaveMasters = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveMaster", new string[] { },
                                       new object[] { });
                var data = SQLHelper<object>.GetListData(employeeOnLeaveMasters, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> SaveEmployeeOnLeaveMaster([FromBody] EmployeeOnLeaveMaster employeeOnLeaveMaster)
        {
            try
            {

                if (employeeOnLeaveMaster.ID <= 0)
                {
                    var employee = SQLHelper<object>.ProcedureToList("spGetCheckDeclareDayOff", new string[] { "@EmployeeID", "ID", "@Year" },
                                                              new object[] { employeeOnLeaveMaster.EmployeeID, employeeOnLeaveMaster.ID, employeeOnLeaveMaster.YearOnleave });
                    var result = SQLHelper<object>.GetListData(employee, 0);

                    if (result.Count > 0)
                    {
                        return Ok(ApiResponseFactory.Success(null, ""));
                    }
                    await _employeeOnLeaveMasterRepo.CreateAsync(employeeOnLeaveMaster);

                }
                else await _employeeOnLeaveMasterRepo.UpdateAsync(employeeOnLeaveMaster);

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        public class EmployeeOnLeaveMasterCheck
        {
            public int? EmployeeID { get; set; }
            public decimal? YearOnleave { get; set; }
        }



        [HttpPost("check-exist")]
        [RequiresPermission("N2,N1")]
        public async Task<IActionResult> CheckExist([FromBody] List<EmployeeOnLeaveMasterCheck> check)
        {
            try
            {
                var employeeIDList = check.Select(x => x.EmployeeID).ToList();
                var yearOnLeaveList = check.Select(x => x.YearOnleave).ToList();


                // Kiểm tra trong database
                var existingDayOff = _employeeOnLeaveMasterRepo.GetAll()
                    .Where(x => employeeIDList.Contains(x.EmployeeID) && yearOnLeaveList.Contains(x.YearOnleave))
                    .Select(x => new
                    {
                        x.ID,
                        x.EmployeeID,
                        x.YearOnleave
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(existingDayOff, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

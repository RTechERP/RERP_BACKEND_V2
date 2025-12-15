using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryApprovedBillLogController : ControllerBase
    {
        private readonly UserRepo _userRepo;
        public HistoryApprovedBillLogController(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("load-data")]
        public IActionResult GetData(int billtype, int employeeID, int warehouseID)
        {
            try
            {
                var result = SQLHelper<dynamic>.ProcedureToList("spGetHistoryIsApprovedBillLog",
                                new string[] { "@BillType", "@EmployeeID", "@WarehouseID" },
                                new object[] { billtype, employeeID, warehouseID });

                var data = SQLHelper<dynamic>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("load-user")]
        public IActionResult GetUser()
        {
            try
            {
                var data = _userRepo.GetAll().Select(x => new { x.ID, x.FullName, x.Code }).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

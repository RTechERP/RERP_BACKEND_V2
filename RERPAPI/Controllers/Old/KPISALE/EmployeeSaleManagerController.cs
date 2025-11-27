using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Old.KPISALE
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeSaleManagerController : ControllerBase
    {
        [HttpGet("get-groupsale")]
        public IActionResult loadGroupSale()
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeTeamSale",null,null);

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-employeesale")]
        public IActionResult loadEmployeeSale(int selectedId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployeebyTeamSale", new string[] { "@EmployeeTeamSaleID" }, new object[] { selectedId });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

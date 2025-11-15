using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryImportExportController : ControllerBase
    {
        [HttpPost("")]
        public IActionResult getReport(HistoryImportExportParam filter)
        {
            try
            {
                if (filter.checkedAll == true)
                {
                    filter.DateStart = new DateTime(1990, 01, 01);
                    filter.DateEnd = new DateTime(9999, 01, 01);
                }
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHistoryImportExport_New",
                    new string[] { "@PageNumber", "@PageSize", "@FilterText", "@DateStart", "@DateEnd", "@Status", "@WarehouseCode" },
                    new object[] { filter.PageNumber, filter.PageSize, filter.FilterText, filter.DateStart, filter.DateEnd, filter.Status, filter.WareHouseCode }
                    );
                var data = SQLHelper<dynamic>.GetListData(result, 0);

                return Ok(new
                {
                    status = 1,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    error = ex.Message
                });
            }
        }
    }
}

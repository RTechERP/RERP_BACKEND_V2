using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportImportExportSaleController : ControllerBase
    {
        [HttpPost("")]

        public IActionResult getReport(ReportImportExportParam filter)
        {
            try
            {
                filter.StartDate = filter.StartDate.Date;

                filter.EndDate = filter.EndDate.Date
                    .AddDays(1)
                    .AddSeconds(-1);
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetDataReportImportExport_New",
                    new string[] { "@StartDate", "@EndDate", "@Find", "@Group", "@WarehouseCode" },
                    new object[] { filter.StartDate, filter.EndDate, filter.Find, filter.Group, filter.WareHouseCode }
                    );

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(result, 0)
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

        [HttpGet("history")]
        public IActionResult getReportDetail(int productsaleID, string warehouseCode)
        {
            try
            {

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHistoryImportExportInventory",
                    new string[] { "@ProductSaleID", "@WarehouseCode" },
                    new object[] { productsaleID, warehouseCode }
                    );

                return Ok(new
                {
                    status = 1,
                    data = result,
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

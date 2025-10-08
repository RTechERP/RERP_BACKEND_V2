using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.SaleWareHouseManagement
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
              
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetDataReportImportExport_New",
                    new string[] { "@StartDate", "@EndDate", "@Find", "@Group", "@WarehouseCode" },
                    new object[] { filter.StartDate, filter.EndDate, filter.Find,filter.Group, filter.WareHouseCode  }
                    );

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("history")]
        public IActionResult getReportDetail(int productsaleID, string warehouseCode)
        {
            try
            {

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHistoryImportExportInventory",
                    new string[] { "@ProductSaleID", "@WarehouseCode"},
                    new object[] { productsaleID, warehouseCode}
                    );

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

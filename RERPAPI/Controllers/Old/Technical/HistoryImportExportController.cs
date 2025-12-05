using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Param.Technical;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryImportExportController : ControllerBase
    {
        [HttpPost("get-bill-import-export-technical")]
        public async Task<ActionResult> GetBillImportExportTechnical([FromBody] GetImportExportTechnicalRequestParam request)
        {
            try
            {
                var billTechnical = SQLHelper<dynamic>.ProcedureToList(
                    "spGetExportImportTechnical",
                    new string[] { "@PageNumber", "@PageSize", "@FilterText", "@DateStart", "@DateEnd", "@Status", "@WarehouseID", "@BillType", "@ReceiverID", "@WarehouseType" },
                    new object[] { request.Page, request.Size, request.FilterText, request.DateStart, request.DateEnd, request.Status, request.WarehouseID ,request.BillType, request.ReceiverID,request.WarehouseType});

                return Ok(new
                {
                    status = 1,
                    billHistoryTechnical = SQLHelper<dynamic>.GetListData(billTechnical, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(billTechnical, 1)
                });

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}

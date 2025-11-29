using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestInvoiceController : ControllerBase
    {
        private readonly RequestInvoiceRepo _requestInvoiceRepo;

        public RequestInvoiceController(RequestInvoiceRepo requestInvoiceRepo)
        {
            _requestInvoiceRepo = requestInvoiceRepo;
        }
        [HttpGet]
        public IActionResult Get(DateTime dateStart, DateTime dateEnd, int warehouseId, string keyWords = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetRequestInvoice", new string[] { "@DateStart", "@DateEnd", "@Keywords", "@WarehouseID" }, new object[] { dateStart, dateEnd, keyWords, warehouseId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-details")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetRequestInvoiceDetailsByID", new string[] { "@RequestInvoiceID" }, new object[] { id });
                List<dynamic> details = SQLHelper<dynamic>.GetListData(list, 0);
                List<dynamic> files = SQLHelper<dynamic>.GetListData(list, 1);
                return Ok(new
                {
                    status = 1,
                    data = details, files
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

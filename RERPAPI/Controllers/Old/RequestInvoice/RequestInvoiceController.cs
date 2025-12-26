using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestInvoiceController : ControllerBase
    {
        private readonly RequestInvoiceRepo _requestInvoiceRepo;
        private readonly POKHFilesRepo _pokhFileRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        public RequestInvoiceController(RequestInvoiceRepo requestInvoiceRepo, POKHFilesRepo pokhFileRepo, ConfigSystemRepo configSystemRepo)
        {
            _requestInvoiceRepo = requestInvoiceRepo;
            _pokhFileRepo = pokhFileRepo;
            _configSystemRepo = configSystemRepo;
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

        [HttpGet("get-request-invoice-by-id")]
        public IActionResult GetRequestInvoiceByID(int id)
        {
            try
            {
                var data = _requestInvoiceRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-pokh-file")]
        public IActionResult GetPOKHFile(int pokhId)
        {
            try
            {
                var data = _pokhFileRepo.GetAll(x=>x.POKHID == pokhId);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-tree-folder-path")]
        public IActionResult GetTreeFolderPath(int requestInvoiceID)
        {
            try
            {

                ConfigSystem config = _configSystemRepo.GetAll(x => x.KeyName == "RequestInvoiceFile").FirstOrDefault();
                if (config == null || string.IsNullOrEmpty(config.KeyValue))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn đường dẫn lưu trên server!"));
                }
                var requestInvoice = _requestInvoiceRepo.GetByID(requestInvoiceID);

                DateTime dateRequest = requestInvoice.CreatedDate.Value;
                string pathServer = config.KeyValue.Trim();
                string pathPattern = $@"{dateRequest.ToString("yyyy")}\T{dateRequest.ToString("MM")}\{requestInvoice.Code}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                return Ok(ApiResponseFactory.Success(pathUpload, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Controllers.CRM;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestInvoiceSummaryController : ControllerBase
    {
        private readonly CustomerRepo _customerRepo;
        public RequestInvoiceSummaryController(CustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }
        [HttpGet("get-request-invoice-summary")]
        public IActionResult GetEmployee(DateTime dateStart, DateTime dateEnd, int customerId, int userId, int status, string keyWords = "")
        {
            try
            {
                var data1 = SQLHelper<dynamic>.ProcedureToList("spGetRequestInvoiceSummary",
                                                new string[] { "@DateStart", "@DateEnd", "@Keywords", "@CustomerID", "@UserID", "@Status" },
                                                new object[] { dateStart, dateEnd, keyWords, customerId, userId, status });
                var data = SQLHelper<dynamic>.GetListData(data1, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-customer")]
        public IActionResult GetCustomer()
        {
            try
            {
                var data = _customerRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

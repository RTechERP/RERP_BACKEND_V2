using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders;

namespace RERPAPI.Controllers.GeneralCategory.PaymentOrders
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentOrderLogController : ControllerBase
    {
        private readonly PaymentOrderRepo _paymentOrderRepo;
        private readonly PaymentOrderLogRepo _paymentOrderLogRepo;
        public PaymentOrderLogController(PaymentOrderRepo paymentOrderRepo, PaymentOrderLogRepo paymentOrderLogRepo)
        {
            _paymentOrderRepo = paymentOrderRepo;
            _paymentOrderLogRepo = paymentOrderLogRepo;
        }

        [HttpGet("get-payment-order")]
        public IActionResult GetPaymentOrder()
        {
            try
            {
                var data = _paymentOrderRepo.GetAll().OrderByDescending(x => x.DateOrder).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-data")]
        public async Task<IActionResult> GetData(int paymentOrderId)
        {
            try
            {
                var param = new
                {
                    PaymentOrderID = paymentOrderId,
                };
                var dataStore = await SqlDapper<object>.ProcedureToListTAsync("spGetPaymentOrderLog", param);
                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-data-new")]
        public async Task<IActionResult> GetDataNew(int paymentOrderId)
        {
            try
            {
                var param = new
                {
                    ID = paymentOrderId,
                };
                var dataStore = await SqlDapper<object>.ProcedureToListTAsync("spGetPaymentOrderLog_New", param);
                return Ok(ApiResponseFactory.Success(dataStore, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

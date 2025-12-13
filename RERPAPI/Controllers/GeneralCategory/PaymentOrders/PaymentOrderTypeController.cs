using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders;

namespace RERPAPI.Controllers.GeneralCategory.PaymentOrders
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentOrderTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;

        private readonly PaymentOrderTypeRepo _orderTypeRepo;
        public PaymentOrderTypeController(IConfiguration configuration, CurrentUser currentUser, PaymentOrderTypeRepo orderTypeRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _orderTypeRepo = orderTypeRepo;
        }


        [HttpGet("")]
        public IActionResult GetAll()
        {
            try
            {
                var types = _orderTypeRepo.GetAll(x => x.IsDelete != true);
                return Ok(ApiResponseFactory.Success(types));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

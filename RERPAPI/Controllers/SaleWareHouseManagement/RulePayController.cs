using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulePayController : ControllerBase
    {
        private readonly RulePayRepo _rulePayRepo;

        public RulePayController(RulePayRepo rulePayRepo)
        {
            _rulePayRepo = rulePayRepo;
        }

        [HttpGet("")]
        public IActionResult getAll()
        {
            try
            {
                List<RulePay> rules = _rulePayRepo.GetAll();

                return Ok(ApiResponseFactory.Success(rules, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
           
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RulePayController : ControllerBase
    {
        private RulePayRepo _rulePayRepo;

        public RulePayController(
            RulePayRepo rulePayRepo
        )
        {
            _rulePayRepo = rulePayRepo;
        }

        [HttpGet("rule-pay")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> getRulePay()
        {
            try
            {
                var data = _rulePayRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
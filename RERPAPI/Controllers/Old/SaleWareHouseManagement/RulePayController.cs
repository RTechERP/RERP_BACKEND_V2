using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulePayController : ControllerBase
    {
        RulePayRepo _rulePayRepo= new RulePayRepo();

        [HttpGet("")]
        public IActionResult getAll()
        {
            try
            {
                List<RulePay> rules = _rulePayRepo.GetAll();

                return Ok(new
                {
                    status=1,
                    data = rules
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                });

            }
           
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using ZXing;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaxCompanyController : ControllerBase
    {
        TaxCompanyRepo _taxCompanyRepo;

        public TaxCompanyController(
            TaxCompanyRepo taxCompanyRepo
        )
        {
            _taxCompanyRepo = taxCompanyRepo;
        }

        [HttpGet("tax-company")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> getTaxCompany()
        {
            try
            {
                var data = _taxCompanyRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
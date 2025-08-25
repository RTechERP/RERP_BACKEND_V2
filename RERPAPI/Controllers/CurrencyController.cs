using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        CurrencyRepo _currencyRepo = new CurrencyRepo();
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<Currency> currencies = _currencyRepo.GetAll();

                return Ok(new
                {
                    status = 1,
                    data = currencies
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
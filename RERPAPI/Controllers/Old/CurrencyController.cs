using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
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
        [HttpGet("currencies")]
        public IActionResult GetAll([FromQuery] string? code)
        {
            try
            {
                List<Currency> currencies = _currencyRepo.GetAll()
                    .Where(c => c.IsDeleted == false)
                    .ToList();

                if (!string.IsNullOrEmpty(code))
                {
                    currencies = currencies
                        .Where(c => c.Code != null && c.Code.Contains(code, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return Ok(ApiResponseFactory.Success(currencies, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // GET: api/currency/123
        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var currency = _currencyRepo.GetByID(id);
                if (currency == null || currency.IsDeleted == true)
                    return NotFound(ApiResponseFactory.Fail(null, "Currency not found."));

                return Ok(ApiResponseFactory.Success(currency, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // POST: api/currency
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Currency currency)
        {
            try
            {
                if (currency == null)
                    return Ok(ApiResponseFactory.Fail(null, "Invalid data"));

                if (currency.ID <= 0)
                {
                    await _currencyRepo.CreateAsync(currency);
                }
                else
                {
                    await _currencyRepo.UpdateAsync(currency);
                }

                return Ok(ApiResponseFactory.Success(currency, "Saved successfully"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



        // DELETE (soft delete): api/currency/123
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                Currency c = _currencyRepo.GetByID(id);
                if (c != null)
                {
                    c.IsDeleted = true;
                    var success = await _currencyRepo.UpdateAsync(c);
                    return Ok(ApiResponseFactory.Success(null, "Deleted successfully"));
                }
                else return NotFound(ApiResponseFactory.Fail(null, "Currency not found."));


            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
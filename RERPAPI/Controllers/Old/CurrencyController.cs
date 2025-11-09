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
        private CurrencyRepo _currencyRepo;
        public CurrencyController(CurrencyRepo currencyRepo)
        {
            _currencyRepo = currencyRepo;
        }   
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<Currency> currencies = _currencyRepo.GetAll(x => x.IsDeleted == false);

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
        [HttpGet("get-by-id")]
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
        [HttpPost("save-data")]
        public async Task<IActionResult> Save([FromBody] Currency currency)
        {
            try
            {
                if (currency == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu của loại tiền"));
                if (_currencyRepo.CheckExist(currency) && currency.IsDeleted != true) return Ok(ApiResponseFactory.Fail(null, "Mã loại tiền đã có trong hệ thống!"));
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

    }
}
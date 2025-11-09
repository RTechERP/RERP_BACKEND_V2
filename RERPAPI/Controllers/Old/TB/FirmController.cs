using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmController : ControllerBase
    {
        private readonly FirmRepo _firmRepo;

        public FirmController(FirmRepo firmRepo)
        {
            _firmRepo = firmRepo;
        }
        [HttpGet("")]
        public IActionResult getDataFirm()
        {
            try
            {
                List<Firm> dataFirm = _firmRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = dataFirm,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataFirm([FromBody] List<Firm> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    if (dto.ID <= 0) await _firmRepo.CreateAsync(dto);
                    else await _firmRepo.UpdateAsync(dto);

                }
                return Ok(new
                {
                    status = 1,
                    message = "Thêm hãng thành công!",

                });

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}

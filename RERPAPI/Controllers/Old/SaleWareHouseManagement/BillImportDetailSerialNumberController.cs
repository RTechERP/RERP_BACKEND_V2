using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillImportDetailSerialNumberController : ControllerBase
    {
        private readonly BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo;
        public BillImportDetailSerialNumberController(BillImportDetailSerialNumberRepo billImportDetailSerialNumberRepo)
        {
            _billImportDetailSerialNumberRepo = billImportDetailSerialNumberRepo;
        }
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                var result = _billImportDetailSerialNumberRepo.GetByID(id);

                return Ok(new
                {
                    status = 1,
                    data = result,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        status = 0,
                        message = ex.Message
                    });
            }
        }

        [HttpPost("save-data")]
        [Authorize]
        public async Task<IActionResult> saveData([FromBody] List<BillImportDetailSerialNumber> data)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var item in data)
                {
                    if (item.ID > 0)
                    {
                        item.UpdatedBy = currentUser.LoginName;
                        await _billImportDetailSerialNumberRepo.UpdateAsync(item);
                    }
                    else
                    {
                        item.CreatedBy = item.UpdatedBy = currentUser.LoginName;
                        await _billImportDetailSerialNumberRepo.CreateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(data, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

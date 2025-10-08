using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillImportDetailSerialNumberController : ControllerBase
    {
        BillImportDetailSerialNumberRepo _billImportDetailSerialNumberRepo = new BillImportDetailSerialNumberRepo();
        [HttpGet("{id}")]
        public IActionResult getDataByID(int id)
        {
            try
            {
                var result = _billImportDetailSerialNumberRepo.GetByID(id);

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu chi tiết theo id hành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

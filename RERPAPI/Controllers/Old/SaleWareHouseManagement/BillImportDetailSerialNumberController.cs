using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Technical;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
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
    }
}

using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSpecializationController : ControllerBase
    {
        CustomerSpecializationRepo _customerSpecializationRepo = new CustomerSpecializationRepo();
        [HttpPost]
        public async Task<IActionResult> Post(CustomerSpecialization model)
        {
            try
            {
                if(model.ID > 0)
                {
                    await _customerSpecializationRepo.UpdateAsync(model);
                }
                else
                {
                    await _customerSpecializationRepo.CreateAsync(model);
                }

                return Ok(ApiResponseFactory.Success("", "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var data = _customerSpecializationRepo.GetAll().Where(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-detail")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                var data = _customerSpecializationRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

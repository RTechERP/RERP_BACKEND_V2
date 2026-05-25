using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Reflection.Metadata.Ecma335;


namespace RERPAPI.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerIndustryController : Controller
    {
        CustomerIndustriesRepo _customerIndustryRepo;

        public CustomerIndustryController(CustomerIndustriesRepo customerIndustryRepo)
        {
            _customerIndustryRepo = customerIndustryRepo;
        }

        [RequiresPermission("N13,N1,N27,N31")]
        [HttpGet()]
        public IActionResult GetCustomerIndustry()
        {
            try
            {
                var customer = _customerIndustryRepo.GetAll(x => x.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = customer
                });
            }catch(Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N13,N1,N27,N31")]
        [HttpPost()]
        public async Task<IActionResult> SaveData ([FromBody] CustomerIndustry customerIndustry ) {
            try
            {
                if(customerIndustry.ID <= 0)
                {
                    var maxSTT = await _customerIndustryRepo.GetNextSTTAsync();
                    customerIndustry.NumberOrder = maxSTT;
                    await _customerIndustryRepo.CreateAsync(customerIndustry);
                }else
                {
                    await _customerIndustryRepo.UpdateAsync(customerIndustry);
                }
                return Ok(new
                {
                    Status = 1,
                    message = "Lưu thành công"
                });
            }catch(Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N13,N1,N27,N31")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn lĩnh vực để xóa"));
                foreach (var item in ids)
                {

                    var project = _customerIndustryRepo.GetByID(item);
                    project.IsDeleted = true;
                    await _customerIndustryRepo.UpdateAsync(project);

                }
                return Ok(ApiResponseFactory.Success(ids, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

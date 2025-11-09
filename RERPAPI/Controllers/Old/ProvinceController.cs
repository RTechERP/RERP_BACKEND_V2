using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvinceController : ControllerBase
    {
        private ProvinceRepo _provinceRepo;
        public ProvinceController(ProvinceRepo provinceRepo)
        {
            _provinceRepo = provinceRepo;
        }
        [HttpGet]
       public async Task<IActionResult> GetProvinces()
        {
           try
            {
                List<Province> provinces = _provinceRepo.GetAll();
                return Ok(new
                {
                    data = provinces,
                    status = 1
                });
            } catch (Exception ex)
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

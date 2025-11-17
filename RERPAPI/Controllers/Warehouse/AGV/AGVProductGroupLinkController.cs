using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Warehouse.AGV
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AGVProductGroupLinkController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AGVProductGroupLinkRepo _groupLinkRepo;
        public AGVProductGroupLinkController(IConfiguration configuration, AGVProductGroupLinkRepo groupLinkRepo)
        {
            _configuration = configuration;
            _groupLinkRepo = groupLinkRepo;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var groupLinks = _groupLinkRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Equals(groupLinks, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AGVProductGroupLink groupLink)
        {
            try
            {
                var validate = _groupLinkRepo.Validate(groupLinks);
                if (validate.status == 0) return BadRequest(validate);
                

                foreach (var groupLink in groupLinks)
                {
                if (groupLink.ID <= 0) await _groupLinkRepo.CreateAsync(groupLink);
                else await _groupLinkRepo.UpdateAsync(groupLink);
                }

                return Ok(ApiResponseFactory.Success(groupLink, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

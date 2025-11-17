using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;

namespace RERPAPI.Controllers.Warehouse.AGV
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AGVProductGroupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AGVProductGroupRepo _groupRepo;

        public AGVProductGroupController(IConfiguration configuration, AGVProductGroupRepo groupRepo)
        {
            _configuration = configuration;
            _groupRepo = groupRepo;
        }


        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var groups = _groupRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(groups, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var group = _groupRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(group, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] AGVProductGroup group)
        {
            try
            {
                var validate = _groupRepo.Validate(group);
                if (validate.status == 0)
                {
                    return BadRequest(validate);
                }

                if (group.ID <= 0) await _groupRepo.CreateAsync(group);
                else await _groupRepo.UpdateAsync(group);

                return Ok(ApiResponseFactory.Success(group, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

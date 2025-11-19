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
        public IActionResult GetAll(int agvProductGroupID, int warehouseID, string? keyword)
        {
            try
            {
                keyword = keyword ?? "";
                var datas = SQLHelper<object>.ProcedureToList("spGetAGVProductGroupLink",
                    new string[] { "@AGVProductGroupID", "@WarehouseID", "@Keyword" },
                    new object[] { agvProductGroupID, warehouseID, keyword });

                var groupLinks = SQLHelper<object>.GetListData(datas, 0);
                return Ok(ApiResponseFactory.Success(groupLinks, ""));
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
                var groupLink = _groupLinkRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(groupLink, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<AGVProductGroupLink> groupLinks)
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

                return Ok(ApiResponseFactory.Success(groupLinks, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
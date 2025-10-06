using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryProductRTCController : ControllerBase
    {
        private const int WAREHOUSE_ID = 1;
        private HistoryProductRTCRepo _historyRepo = new HistoryProductRTCRepo();

        [Authorize]
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                DateTime dateStart = new DateTime(1900, 01, 01);
                DateTime dateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                string status = "4,7";
                var historys = SQLHelper<object>.ProcedureToList("spGetHistoryProduct_New",
                                new string[] { "@DateStart", "@DateEnd", "@Status", "@WarehouseID", "@UserID" },
                                new object[] { dateStart, dateEnd, status, WAREHOUSE_ID, currentUser.ID });

                var data = SQLHelper<object>.GetListData(historys, 0);
                var borrows = data.Where(x => x.Status == 7).ToList();
                var returns = data.Where(x => x.Status == 4).ToList();
                return Ok(ApiResponseFactory.Success(new { borrows, returns }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<HistoryProductRTC> historyProducts)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                //_historyRepo.SetClaim(claims);
                foreach (var item in historyProducts)
                {
                    if (currentUser.ID != item.PeopleID) continue;
                    await _historyRepo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(historyProducts, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Warehouses.AGV;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Warehouse.AGV
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AGVHistoryProductController : ControllerBase
    {
        const int WAREHOUSE_ID = 1;
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;
        private readonly AGVHistoryProductRepo _historyProductRepo;

        public AGVHistoryProductController(IConfiguration configuration,CurrentUser currentUser, AGVHistoryProductRepo historyProductRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _historyProductRepo = historyProductRepo;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var historys = _historyProductRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(historys, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<AGVHistoryProduct> historyProducts)
        {
            try
            {
                _currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey"));
                //if (_currentUser.ID <= 0)
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!"));
                //}

                int[] userIDAdminWarehouse = new int[] { 24, 1434, 88, 1534 };
                bool isAdminWarehouse = userIDAdminWarehouse.Contains(_currentUser.ID);
                foreach (var item in historyProducts)
                {
                    if (item.ID <= 0)
                    {

                        item.Status = 7;
                        if (_currentUser.IsAdmin || isAdminWarehouse)
                        {
                            item.AdminConfirm = true;
                            item.Status = 1;
                        }
                        item.WarehouseID = WAREHOUSE_ID;
                        await _historyProductRepo.CreateAsync(item);
                    }
                    else await _historyProductRepo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(historyProducts, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

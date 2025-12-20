using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity.Systems;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Systems
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MenuAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CurrentUser _currentUser;

        private readonly MenuAppRepo _menuRepo;
        private readonly MenuAppUserGroupLinkRepo _menuLinkRepo;

        public MenuAppController(IConfiguration configuration, CurrentUser currentUser, MenuAppRepo menuRepo, MenuAppUserGroupLinkRepo menuLinkRepo)
        {
            _configuration = configuration;
            _currentUser = currentUser;
            _menuRepo = menuRepo;
            _menuLinkRepo = menuLinkRepo;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var connection = new SqlConnection(_configuration.GetValue<string>("ConnectionString") ?? "");
                var param = new { Keyword = "" };
                var data = await connection.QueryMultipleAsync("spGetMenu", param, commandType: System.Data.CommandType.StoredProcedure);

                var menus = (await data.ReadAsync()).ToList();

                return Ok(ApiResponseFactory.Success(menus, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var menu = _menuRepo.GetByID(id);
                var menuLinks = _menuLinkRepo.GetAll(x => x.MenuAppID == menu.ID && x.IsDeleted != true);

                return Ok(ApiResponseFactory.Success(new { menu, menuLinks }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] MenuAppDTO menu)
        {
            try
            {
                if (menu.ID <= 0) await _menuRepo.CreateAsync(menu);
                else await _menuRepo.UpdateAsync(menu);

                //Xóa hết link cũ
                var menuLinks = _menuLinkRepo.GetAll(x => x.MenuAppID == menu.ID && x.IsDeleted != true);
                foreach (var item in menuLinks)
                {
                    item.IsDeleted = true;
                    await _menuLinkRepo.UpdateAsync(item);
                }

                if (menu.IsDeleted != true)
                {
                    menu.MenuAppUserGroupLinks.ForEach(x =>
                    {
                        x.MenuAppID = menu.ID;
                    });

                    await _menuLinkRepo.CreateRangeAsync(menu.MenuAppUserGroupLinks);
                }

                return Ok(ApiResponseFactory.Success(menu, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

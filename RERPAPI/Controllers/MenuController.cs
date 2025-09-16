using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.IRepo;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [ApiKeyAuthorize]
    public class MenuController : ControllerBase
    {
        //Response _response = new Response();
        MenuRepo _menuRepo = new MenuRepo();

        //[RequiresPermission("N42")]
        //[ApiKeyAuthorize]
        [HttpGet("menus/{parentid}")]
        public IActionResult GetAll(int parentid)
        {
            try
            {
                Menu menu = _menuRepo.GetByID(parentid);
                var menus = ObjectMapper.MapTo<MenuDTO>(menu);
                menus.MenuChilds = _menuRepo.GetAll(x => x.ParentID == menu.ID);
                return Ok(ApiResponseFactory.Success(menus, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] Menu menu)
        {
            try
            {

                if (menu.ID <= 0) await _menuRepo.CreateAsync(menu);
                else await _menuRepo.UpdateAsync(menu);
                return Ok(ApiResponseFactory.Success(menu, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}

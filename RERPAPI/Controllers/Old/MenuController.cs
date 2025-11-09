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

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]

    //[ApiKeyAuthorize]
    public class MenuController : ControllerBase
    {
        //Response _response = new Response();
        MenuRepo _menuRepo ;
        public MenuController(MenuRepo menuRepo)
        {
            this._menuRepo = menuRepo;
        }
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


        [HttpGet("menus/parent")]
        public IActionResult GetAllParent()
        {
            try
            {
                var menuparents = _menuRepo.GetAll(x => x.ParentID == 0).ToList();
                //return Ok(new
                //{
                //    status = 1,
                //    data = menuparents
                //});
                return Ok(ApiResponseFactory.Success(menuparents, ""));

            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    mesage = ex.Message,
                //    error = ex.ToString()
                //});
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
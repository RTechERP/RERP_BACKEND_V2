using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Security.Cryptography;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyUnitController : ControllerBase
    {
        OfficeSupplyUnitRepo osurepo = new OfficeSupplyUnitRepo();

        [HttpGet("getdataofficesupplyunit")]
        public IActionResult GetDataOfficeSupplyUnini()
        {
            try
            {
                List<OfficeSupplyUnit> result = SQLHelper<OfficeSupplyUnit>.FindAll();
                var data = result.Where(x => x.IsDeleted == false).ToList();
                return Ok(new
                {
                    status = 1,
                    data=data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }        
        }

        [HttpGet("getbyidofficesupplyunit")]
        public IActionResult GetByIDOfficeSupplyUnit(int id)
        {
            try
            {
                OfficeSupplyUnit dst = osurepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = dst
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        //danh sách tính
        [HttpPost("savedatofficesupplyunit")]
        public async Task<IActionResult> SaveDST([FromBody] OfficeSupplyUnit dst)
        {
            try
            {
                if (dst.ID <= 0)
                {
                    dst.IsDeleted = false;
                    await osurepo.CreateAsync(dst);
                }
                else
                {                  
                    osurepo.UpdateFieldsByID(dst.ID, dst);
                }              
                return Ok(new
                {
                    status = 1,
                    data = dst
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("deleteofficesupplyunit")]
        public async Task<IActionResult> DeleteOfficeSupplyUnit([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest("Danh sách ID không hợp lệ.");

                foreach (var id in ids)
                {
                    var item = osurepo.GetByID(id);
                    if (item != null)
                    {
                        item.IsDeleted = true; // Gán trường IsDeleted thành true
                        /* await off.UpdateAsync(item);*/
                        osurepo.UpdateFieldsByID(id, item);/* // Cập nhật lại mục*/
                    }
                }
                return Ok(new
                {
                    status = 1,
                    message = "Đã xóa thành công"
                }) ;
            }
            catch (Exception ex)
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

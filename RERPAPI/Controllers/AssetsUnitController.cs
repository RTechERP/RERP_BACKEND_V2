using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsUnitController : ControllerBase
    {
        UnitRepo unitrepo = new UnitRepo();

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                var units = unitrepo.GetAll().Where(x => !x.IsDeleted).ToList();
             
                return Ok(new
                {
                    status = 1,
                    data = units
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
     
        [HttpPost("savedataa")]
        public async Task<IActionResult> SaveData([FromBody] List<UnitCount> uni)
        {
            try
            {
                if (uni != null && uni.Any())
                {
                    foreach (var item in uni)
                    {
                        if (item.ID > 0)
                        {
                            if (item.IsDeleted==false)
                            {
                                unitrepo.UpdateFieldsByID(item.ID, new UnitCount
                                {
                                    IsDeleted = true
                                });
                            }
                            else
                            {
                                unitrepo.UpdateFieldsByID(item.ID, item);
                            }
                        }
                        else
                        {
                            unitrepo.Create(item);
                        }
                    }
                }
                return Ok(new { status = 1 });
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

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
        UnitRepo unitRepo = new UnitRepo();

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                var units = unitRepo.GetAll().Where(x => !x.IsDeleted).ToList();
             
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
     
        [HttpPost("saveData")]
        public async Task<IActionResult> SaveData([FromBody] List<UnitCount> units)
        {
            try
            {
                if (units != null && units.Any())
                {
                    foreach (var item in units)
                    {
                        if (item.ID > 0)
                        {
                            if (item.IsDeleted==false)
                            {
                                unitRepo.UpdateFieldsByID(item.ID, new UnitCount
                                {
                                    IsDeleted = true
                                });
                            }
                            else
                            {
                                unitRepo.UpdateFieldsByID(item.ID, item);
                            }
                        }
                        else
                        {
                            unitRepo.Create(item);
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

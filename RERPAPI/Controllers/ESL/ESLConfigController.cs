using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.ESL;
using System;
using System.Linq;

namespace RERPAPI.Controllers.ESL
{
    [Route("api/[controller]")]
    [ApiController]
    public class ESLConfigController : ControllerBase
    {
        private readonly ESLConfigRepo _configRepo;

        public ESLConfigController(ESLConfigRepo configRepo)
        {
            _configRepo = configRepo;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                var configs = _configRepo.GetAll(x => x.IsDeleted != true).ToList();
                return Ok(new
                {
                    status = 1,
                    data = configs
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("save")]
        public IActionResult Save([FromBody] ESLConfig item)
        {
            try
            {
                if (item.ID == 0)
                {
                    item.UpdatedDate = DateTime.Now;
                    _configRepo.Create(item);
                }
                else
                {
                    var exist = _configRepo.GetByID(item.ID);
                    if (exist != null && exist.ID > 0)
                    {
                        exist.ConfigKey = item.ConfigKey;
                        exist.ConfigValue = item.ConfigValue;
                        exist.Description = item.Description;
                        exist.UpdatedDate = DateTime.Now;
                        _configRepo.Update(exist);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("delete")]
        public IActionResult Delete([FromBody] int id)
        {
            try
            {
                var exist = _configRepo.GetByID(id);
                if (exist != null && exist.ID > 0)
                {
                    exist.IsDeleted = true;
                    _configRepo.Update(exist);
                    return Ok(new { status = 1, message = "Xóa thành công" });
                }
                return Ok(new { status = 0, message = "Bản ghi không tồn tại" });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}

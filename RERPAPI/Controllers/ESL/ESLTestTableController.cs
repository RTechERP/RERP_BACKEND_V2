using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities.ESL;
using RERPAPI.Repo.GenericEntity.ESL;
using System;
using System.Linq;

namespace RERPAPI.Controllers.ESL
{
    [Route("api/[controller]")]
    [ApiController]
    public class ESLTestTableController : ControllerBase
    {
        private readonly ESLTestTableRepo _testTableRepo;

        public ESLTestTableController(ESLTestTableRepo testTableRepo)
        {
            _testTableRepo = testTableRepo;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                var tables = _testTableRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data = tables
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
        public IActionResult Save([FromBody] ESLTestTable item)
        {
            try
            {
                if (item.ID == 0)
                {
                    item.CreatedDate = DateTime.Now;
                    _testTableRepo.Create(item);
                }
                else
                {
                    var exist = _testTableRepo.GetByID(item.ID);
                    if (exist != null && exist.ID > 0)
                    {
                        exist.TestTableName = item.TestTableName;
                        exist.Barcode = item.Barcode;
                        exist.Description = item.Description;
                        exist.IsActive = item.IsActive;
                        exist.UpdatedDate = DateTime.Now;
                        _testTableRepo.Update(exist);
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
                var exist = _testTableRepo.GetByID(id);
                if (exist != null && exist.ID > 0)
                {
                    _testTableRepo.Delete(id);
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

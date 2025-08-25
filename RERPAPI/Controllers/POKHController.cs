using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POKHController : ControllerBase
    {
        POKHRepo _pokhRepo = new POKHRepo();
        #region Lấy tất cả POKH
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<POKH> result = _pokhRepo.GetAll();

                return Ok(new
                {
                    status = 1,
                    data = result
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
        #endregion
    }
}
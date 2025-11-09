using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitGuestTypeController : ControllerBase
    {
        private readonly VisitGuestTypeRepo _repo;
        public VisitGuestTypeController(VisitGuestTypeRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                List<VisitGuestType> items = _repo.GetAll(x => !x.IsDeleted);
                return Ok(new { status = 1, data = items });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }

        [HttpGet("getbyid")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var item = _repo.GetByID(id);
                return Ok(new { status = 1, data = item });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] VisitGuestType request)
        {
            try
            {
                request.IsDeleted = false;
                if (request.ID <= 0) await _repo.CreateAsync(request);
                else await _repo.UpdateAsync(request);
                return Ok(new { status = 1, data = request });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var item = _repo.GetByID(id);
                if (item == null || item.ID <= 0) return Ok(new { status = 1, message = "Not found" });
                item.IsDeleted = true;
                _repo.Update(item);
                return Ok(new { status = 1, message = "Deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
    }
}



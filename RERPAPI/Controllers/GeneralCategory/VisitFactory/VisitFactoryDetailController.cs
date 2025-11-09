using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitFactoryDetailController : ControllerBase
    {
        private readonly VisitFactoryDetailRepo _repo;
        public VisitFactoryDetailController(VisitFactoryDetailRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("getall")]
        public IActionResult GetAll(int? visitFactoryID)
        {
            try
            {
                List<VisitFactoryDetail> items = visitFactoryID.HasValue
                    ? _repo.GetAll(x => x.VisitFactoryID == visitFactoryID.Value && !x.IsDeleted)
                    : _repo.GetAll(x => !x.IsDeleted);
                var data = items.Select(x => new VisitFactoryDetailDTO
                {
                    Id = x.ID,
                    VisitFactoryId = x.VisitFactoryID,
                    EmployeeId = x.EmployeeID,
                    FullName = x.FullName,
                    Company = x.Company,
                    Position = x.Position,
                    Phone = x.Phone,
                    Email = x.Email,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted
                }).ToList();
                return Ok(new { status = 1, data });
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
                var x = _repo.GetByID(id);
                if (x == null) return Ok(new { status = 1, data = (object?)null });
                var dto = new VisitFactoryDetailDTO
                {
                    Id = x.ID,
                    VisitFactoryId = x.VisitFactoryID,
                    EmployeeId = x.EmployeeID,
                    FullName = x.FullName,
                    Company = x.Company,
                    Position = x.Position,
                    Phone = x.Phone,
                    Email = x.Email,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    IsDeleted = x.IsDeleted
                };
                return Ok(new { status = 1, data = dto });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] VisitFactoryDetailDTO request)
        {
            try
            {
                if (request == null) return BadRequest(new { status = 0, message = "Request is null" });
                if (request.Id <= 0)
                {
                    if (request.VisitFactoryId <= 0) return BadRequest(new { status = 0, message = "VisitFactoryID is required" });
                    var entity = new VisitFactoryDetail
                    {
                        VisitFactoryID = request.VisitFactoryId,
                        EmployeeID = request.EmployeeId,
                        FullName = string.IsNullOrWhiteSpace(request.FullName) ? string.Empty : request.FullName,
                        Company = request.Company,
                        Position = request.Position,
                        Phone = request.Phone,
                        Email = request.Email,
                        CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy) ? "system" : request.CreatedBy,
                        CreatedDate = request.CreatedDate == default ? DateTime.UtcNow : request.CreatedDate,
                        UpdatedBy = request.UpdatedBy,
                        UpdatedDate = request.UpdatedDate,
                        IsDeleted = false
                    };
                    await _repo.CreateAsync(entity);
                    var dto = new VisitFactoryDetailDTO
                    {
                        Id = entity.ID,
                        VisitFactoryId = entity.VisitFactoryID,
                        EmployeeId = entity.EmployeeID,
                        FullName = entity.FullName,
                        Company = entity.Company,
                        Position = entity.Position,
                        Phone = entity.Phone,
                        Email = entity.Email,
                        CreatedBy = entity.CreatedBy,
                        CreatedDate = entity.CreatedDate,
                        UpdatedBy = entity.UpdatedBy,
                        UpdatedDate = entity.UpdatedDate,
                        IsDeleted = entity.IsDeleted
                    };
                    return Ok(new { status = 1, data = dto });
                }
                else
                {
                    var update = new VisitFactoryDetail
                    {
                        ID = request.Id,
                        VisitFactoryID = request.VisitFactoryId,
                        EmployeeID = request.EmployeeId,
                        FullName = request.FullName,
                        Company = request.Company,
                        Position = request.Position,
                        Phone = request.Phone,
                        Email = request.Email,
                        CreatedBy = request.CreatedBy,
                        CreatedDate = request.CreatedDate,
                        UpdatedBy = request.UpdatedBy,
                        UpdatedDate = request.UpdatedDate,
                        IsDeleted = false
                    };
                    await _repo.UpdateAsync(update);
                    return Ok(new { status = 1, data = request });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }

        [HttpPost("delete")]
        public IActionResult DeleteMany([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(new { status = 0, message = "Ids is empty" });
                }

                var distinctIds = ids.Distinct().ToList();
                int deletedCount = 0;
                foreach (var id in distinctIds)
                {
                    var entity = _repo.GetByID(id);
                    if (entity == null || entity.ID <= 0 || entity.IsDeleted) continue;
                    entity.IsDeleted = true;
                    entity.UpdatedDate = DateTime.UtcNow;
                    _repo.Update(entity);
                    deletedCount++;
                }

                return Ok(new { status = 1, message = "Deleted", deleted = deletedCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
    }
}



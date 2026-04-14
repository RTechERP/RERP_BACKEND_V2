using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiveSRatingTicketController : ControllerBase
    {
        private readonly FiveSRatingTicketRepo _ticketRepo;
        private readonly FiveSErrorRepo _fiveSErrorRepo;
        private readonly FiveSRatingDetailRepo _fiveSRatingDetailRepo;
        private CurrentUser _currentUser;

        public FiveSRatingTicketController(
            FiveSRatingTicketRepo ticketRepo,
            FiveSErrorRepo fiveSErrorRepo,
            FiveSRatingDetailRepo fiveSRatingDetailRepo,
            CurrentUser currentUser)
        {
            _ticketRepo = ticketRepo;
            _fiveSErrorRepo = fiveSErrorRepo;
            _fiveSRatingDetailRepo = fiveSRatingDetailRepo;
            _currentUser = currentUser;
        }

        [HttpGet("get-by-session")]
        [Authorize]
        public async Task<IActionResult> GetBySession(int sessionId)
        {
            try
            {
                var result = await _ticketRepo.GetBySessionIDAsync(sessionId);
                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách phiếu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-all")]
        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var result = _ticketRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách phiếu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] FiveSRatingTicket item)
        {
            try
            {
                if (item == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                
                int result = 0;
                if (item.ID > 0)
                {
                    result = await _ticketRepo.UpdateAsync(item);
                }
                else
                {
                    if (string.IsNullOrEmpty(item.TicketCode))
                    {
                        item.TicketCode = await _ticketRepo.GenerateTicketCodeAsync(item.Rating5SID ?? 0);
                    }
                    result = await _ticketRepo.CreateAsync(item);
                }

                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                else
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Create ticket + pre-initialize detail rows for selected departments
        /// </summary>
        [Authorize]
        [HttpPost("save-with-details")]
        public async Task<IActionResult> SaveWithDetails([FromBody] FiveSRatingTicketWithDetailsDTO dto)
        {
            try
            {
                if (dto == null || dto.DepartmentIDs == null || dto.DepartmentIDs.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn ít nhất 1 phòng ban"));

                // 1. Create Ticket
                var ticket = new FiveSRatingTicket
                {
                    Rating5SID = dto.Rating5SID,
                    EmployeeRating1ID = dto.EmployeeRating1ID,
                    EmployeeRating2ID = dto.EmployeeRating2ID,
                    Note = dto.Note,
                    IsDeleted = false
                };
                ticket.TicketCode = await _ticketRepo.GenerateTicketCodeAsync(ticket.Rating5SID ?? 0);

                // Use sync Create so ticket.ID is populated after SaveChanges
                int ticketResult = _ticketRepo.Create(ticket);
                if (ticketResult == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Tạo phiếu thất bại"));

                // 2. Get all active errors
                var errors = _fiveSErrorRepo.GetAll(e => e.IsDeleted != true);

                // 3. Generate detail rows: Error × Department
                var details = new List<FiveSRatingDetail>();
                foreach (var error in errors)
                {
                    foreach (var deptId in dto.DepartmentIDs)
                    {
                        details.Add(new FiveSRatingDetail
                        {
                            FiveSErrorID = error.ID,
                            FiveSDepartmentID = deptId,
                            Rating5SID = dto.Rating5SID,
                            FiveSRatingTicketID = ticket.ID,
                            EmployeeRating1ID = dto.EmployeeRating1ID,
                            EmployeeRating2ID = dto.EmployeeRating2ID,
                            IsDeleted = false
                        });
                    }
                }

                // 4. Bulk insert details
                if (details.Count > 0)
                {
                    _fiveSRatingDetailRepo.CreateRange(details);
                }

                return Ok(ApiResponseFactory.Success(ticket.ID, "Tạo phiếu và chi tiết thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] FiveSRatingTicket item)
        {
            try
            {
                if (item == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                
                item.IsDeleted = true;
                int result = await _ticketRepo.UpdateAsync(item);

                if (result > 0)
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                else
                    return BadRequest(ApiResponseFactory.Fail(null, "Xóa dữ liệu không thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

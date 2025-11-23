using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingTypeController : ControllerBase
    {
        private readonly MeetingTypeRepo _meetingTypeRepo;
        public MeetingTypeController(MeetingTypeRepo meetingTypeRepo)
        {
            _meetingTypeRepo = meetingTypeRepo;
        }
        #region hoang hai
        /// <summary>
        /// Lấy danh sách tất cả loại cuộc họp
        /// </summary>
        [HttpGet("meetingtypes")]
        public IActionResult GetAll()
        {
            try
            {
                var meetingtypes = _meetingTypeRepo.GetAll(p => p.IsDelete == false);
                return Ok(ApiResponseFactory.Success(meetingtypes, "Lấy dữ liệu thành công "));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] MeetingType meetingtype)
        {
            try
            {

                if (meetingtype.ID <= 0)
                {
                    meetingtype.IsDelete = false;
                    await _meetingTypeRepo.CreateAsync(meetingtype);
                }
                else await _meetingTypeRepo.UpdateAsync(meetingtype);
                return Ok(ApiResponseFactory.Success(meetingtype, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("checkmeetingtype/{id}/{typecode}")]
        public async Task<IActionResult> CheckMeetingType(int id, string typecode)
        {
            try
            {
                bool check = false;

                var meetingType = _meetingTypeRepo.GetAll()
                    .Where(x => x.ID != id &&
                               x.TypeCode.ToLower() == typecode.ToLower() &&
                               x.IsDelete == false);

                if (meetingType.Count() > 0) check = true;

                return Ok(ApiResponseFactory.Success(check, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<MeetingType> data)
        {
            try
            {
                foreach (var dto in data)
                {
                    string message = "";
                    if (dto.IsDelete != true)
                    {
                        if (!_meetingTypeRepo.Validate(dto, out message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, message));
                        }
                        if (_meetingTypeRepo.CheckTypeCodeExists(dto.TypeCode, dto.ID))
                        {
                            return Ok(new { status = 2, message = $"Mã loại cuộc họp '{dto.TypeCode}' đã tồn tại!" });
                        }
                    }
                    if (dto.ID > 0)
                    {
                        await _meetingTypeRepo.UpdateAsync(dto);
                    }
                    else
                    {
                        await _meetingTypeRepo.CreateAsync(dto);
                    }

                }
                return Ok(ApiResponseFactory.Success(null, "Lưu loại cuộc họp thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

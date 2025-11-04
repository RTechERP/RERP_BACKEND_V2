using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingTypeController : ControllerBase
    {
        private readonly MeetingTypeRepo meetingtypeRepo;

        public MeetingTypeController()
        {
            meetingtypeRepo = new MeetingTypeRepo();
        }

        /// <summary>
        /// Lấy danh sách tất cả loại cuộc họp
        /// </summary>
        [HttpGet("meetingtypes")]
        public IActionResult GetAll()
        {
            try
            {
                var meetingtypes = meetingtypeRepo.GetAll(p => p.IsDelete == false);
                return Ok(ApiResponseFactory.Success(meetingtypes, ""));
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

                if (meetingtype.ID <= 0) await meetingtypeRepo.CreateAsync(meetingtype);
                else await meetingtypeRepo.UpdateAsync(meetingtype);
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

                var meetingType = meetingtypeRepo.GetAll()
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
    }
}
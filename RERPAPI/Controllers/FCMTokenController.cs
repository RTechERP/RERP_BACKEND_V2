using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FCMTokenController : ControllerBase
    {
        private readonly FcmTokenRepo _fcmTokenRepo;
        public FCMTokenController(FcmTokenRepo fcmTokenRepo)
        {
            _fcmTokenRepo = fcmTokenRepo;
            _fcmTokenRepo = fcmTokenRepo;
        }
        [Authorize]
        [HttpPost("insert-token")]
        public async Task<IActionResult> InsertOrUpdateToken([FromBody] string token, string deviceID)
        {
            try
            {

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token cannot be empty.");
                }
                var existingTokens = _fcmTokenRepo.GetTokensByEmployeeID(currentUser.EmployeeID);
                if (existingTokens.Contains(token))
                {
                    return Ok("Token already exists.");
                }
                var newToken = new Model.Entities.FcmToken
                {
                    ID = 0,
                    Token = token,
                    EmployeeID = currentUser.EmployeeID,
                    DeviceID = deviceID

                };
                await _fcmTokenRepo.CreateAsync(newToken);
                return Ok(ApiResponseFactory.Success(newToken, "Token inserted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [Authorize]
        [HttpPost("delete-token")]
        public async Task<IActionResult> DeleteToken([FromBody] string token, string deviceID)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token cannot be empty.");
                }
                var existingTokens = _fcmTokenRepo.GetTokensByEmployeeID(currentUser.EmployeeID);
                if (!existingTokens.Contains(token))
                {
                    return NotFound("Token not found.");
                }
                var tokenEntity = _fcmTokenRepo.GetAll().FirstOrDefault(t => t.Token == token && t.EmployeeID == currentUser.EmployeeID && t.DeviceID  == deviceID);
                if (tokenEntity != null)
                {
                    await _fcmTokenRepo.DeleteAsync(tokenEntity.ID);
                    return Ok(ApiResponseFactory.Success(tokenEntity, "Token deleted successfully."));
                }
                return NotFound("Token not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }
    }
}

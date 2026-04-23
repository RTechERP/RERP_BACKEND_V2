using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO.MobileDTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RERPAPI.Controllers.AppMobile
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeMobileController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly RTCContext _context;
        private readonly IConfiguration _configuration;
        EmployeeOverTimeRepo _employeeOverTimeRepo;
        //UserRepo _userRepo = new UserRepo();
        vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly RoleConfig _roleConfig;
        private readonly EmployeePayrollDetailRepo _employeePayrollDetailRepo;

        private readonly EmployeeOnLeaveRepo _onLeaveRepo;
        private readonly EmployeeWFHRepo _wfhRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly EmailHelper _emailHelper;
        private readonly FcmTokenRepo _fcmTokenRepo;

        //IRabbitMqPublisher _publisher;
        public HomeMobileController(IOptions<JwtSettings> jwtSettings, RTCContext context,
            IConfiguration configuration, EmployeeOnLeaveRepo onLeaveRepo, vUserGroupLinksRepo vUserGroupLinksRepo, EmployeeWFHRepo employeeWFHRepo, ConfigSystemRepo configSystemRepo, EmployeeOverTimeRepo employeeOverTimeRepo, RoleConfig roleConfig, EmployeePayrollDetailRepo employeePayrollDetailRepo, EmailHelper emailHelper, FcmTokenRepo fcmTokenRepo)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _configuration = configuration;
            _onLeaveRepo = onLeaveRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _wfhRepo = employeeWFHRepo;
            _configSystemRepo = configSystemRepo;
            _employeeOverTimeRepo = employeeOverTimeRepo;
            _roleConfig = roleConfig;
            _employeePayrollDetailRepo = employeePayrollDetailRepo;
            //_publisher = publisher;
            _emailHelper = emailHelper;
            _fcmTokenRepo = fcmTokenRepo;
        }
        [HttpPost("login-mobile")]
        public async Task<IActionResult> Login([FromBody] UserMobileDTO user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.LoginName) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đăng nhập và Mật khẩu!"));
                }

                //1. Check user
                string loginName = user.LoginName ?? "";
                string password = MaHoaMD5.EncryptPassword(user.PasswordHash ?? "");
                //password = user.PasswordHash;
                var login = SQLHelper<object>.ProcedureToList("spLogin", new string[] { "@LoginName", "@Password" }, new object[] { loginName, password });
                var hasUsers = SQLHelper<object>.GetListData(login, 0);

                if (hasUsers.Count <= 0 || hasUsers[0].ID <= 0)
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Sai tên đăng nhập hoặc mật khẩu!"));
                }

                var hasUser = SQLHelper<object>.GetListData(login, 0)[0];
                int employeeId = hasUser.EmployeeID;

                // ================================
                // 🔥 LƯU / UPDATE FCM TOKEN
                // ================================
                if (!string.IsNullOrWhiteSpace(user.FcmToken) && !string.IsNullOrWhiteSpace(user.DeviceID))
                {
                    var existing = _fcmTokenRepo.GetAll(x => x.DeviceID == user.DeviceID).FirstOrDefault();

                    if (existing != null)
                    {
                        // Device đã tồn tại → update token
                        existing.Token = user.FcmToken;
                        existing.EmployeeID = employeeId;
                        await _fcmTokenRepo.UpdateAsync(existing);
                    }
                    else
                    {
                        // Device mới → insert
                        var newToken = new FcmToken
                        {
                            Token = user.FcmToken,
                            EmployeeID = employeeId,
                            DeviceID = user.DeviceID
                        };

                        await _fcmTokenRepo.CreateAsync(newToken);
                    }
                }


                //2. Tạo Claims
                var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,hasUser.ID.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName,hasUser.LoginName ?? ""),
                        new Claim("iscandidate", _jwtSettings.IsCandidate.ToString().ToLower())
                    };

                var dictionary = (IDictionary<string, object>)hasUser;
                foreach (var item in dictionary)
                {
                    if (item.Key.ToLower() == "passwordhash") continue;
                    var claim = new Claim(item.Key.ToLower(), item.Value?.ToString() ?? "");
                    claims.Add(claim);
                }


                //3. Tạo token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims.ToArray(),
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    signingCredentials: creds
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


                return Ok(new
                {
                    access_token = tokenString,
                    expires = token.ValidTo.AddHours(+7)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message + "\n"));
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly RTCContext _context;

        //UserRepo _userRepo = new UserRepo();
        vUserGroupLinksRepo _vUserGroupLinksRepo = new vUserGroupLinksRepo();

        public HomeController(IOptions<JwtSettings> jwtSettings, RTCContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                //1. Check user
                string loginName = user.LoginName ?? "";
                string password = MaHoaMD5.EncryptPassword(user.PasswordHash ?? "");

                var login = SQLHelper<object>.ProcedureToList("spLogin", new string[] { "@LoginName", "@Password" }, new object[] { loginName, password });
                var hasUsers = SQLHelper<object>.GetListData(login, 0);

                if (hasUsers.Count <= 0 || hasUsers[0].ID <= 0)
                {
                    //_response.status = 0;
                    //_response.message = "Sai tên đăng nhập hoặc mật khẩu!";
                    return Unauthorized(ApiResponseFactory.Fail(null, "Sai tên đăng nhập hoặc mật khẩu!"));
                }

                var hasUser = SQLHelper<object>.GetListData(login, 0)[0];

                //2. Tạo Claims

                var claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Sub,hasUser.ID.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName,hasUser.LoginName ?? ""),
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
                _context.CurrentUser = ObjectMapper.GetCurrentUser(claims.ToDictionary(x => x.Type, x => x.Value));
                return Ok(new
                {
                    access_token = tokenString,
                    expires = token.ValidTo
                });
            }
            catch (Exception ex)
            {
                //_response.status = 0;
                //_response.message = ex.Message;
                //_response.error = ex.ToString();
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [Authorize]
        [ApiKeyAuthorize]
        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                return Ok(ApiResponseFactory.Success(currentUser, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

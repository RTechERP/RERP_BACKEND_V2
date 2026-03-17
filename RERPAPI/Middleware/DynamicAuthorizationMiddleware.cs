using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using RERPAPI.Attributes;
using RERPAPI.IRepo;
using RERPAPI.Model.Common;
using System.Security.Claims;
using System.Text.Json;

namespace RERPAPI.Middleware
{
    public class DynamicAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;
        public DynamicAuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiKey = configuration["ApiKey"] ?? throw new Exception("ApiKey missing in config");
        }
        public async Task InvokeAsync(HttpContext context, IUserPermissionService permissionService)
        {
            var endpoint = context.GetEndpoint();

            // 🔹 Check xem có gắn [ApiKeyAuthorize]
            var apiKeyAttr = endpoint?.Metadata.GetMetadata<ApiKeyAuthorizeAttribute>();
            if (apiKeyAttr != null)
            {
                bool isApiKey = context.Request.Headers.TryGetValue("x-api-key", out var apiKey);
                if (!isApiKey || apiKey != _apiKey)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var response = JsonSerializer.Serialize(ApiResponseFactory.Unauthorized("Invalid or missing API Key!"));
                    await context.Response.WriteAsync(response);

                    //await context.Response.WriteAsync("Invalid or missing API Key");
                    return;
                }

                context.Items["AuthType"] = "ApiKey";
                await _next(context);
                return;
            }

            // 🔹 Check xem có gắn [RequiresPermission]
            var permissionAttributes = endpoint?.Metadata.GetOrderedMetadata<RequiresPermissionAttribute>();
            var authorizeAttribute = endpoint?.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

            //if (permissionAttributes != null && permissionAttributes.Count > 0)
            if (authorizeAttribute != null && authorizeAttribute.Count > 0)
            {

                bool? isAuthen = context.User.Identity?.IsAuthenticated;
                //Check có token không
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var response = JsonSerializer.Serialize(ApiResponseFactory.Unauthorized("Vui lòng đăng nhập!"));
                    await context.Response.WriteAsync(response);

                    //await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                //Check còn hạn không
                long expClaims = Convert.ToInt64(context.User.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);
                DateTime expires = DateTimeOffset.FromUnixTimeSeconds(expClaims).UtcDateTime.AddHours(+7);

                expires = new DateTime(expires.Year, expires.Month, expires.Day, expires.Hour, expires.Minute, 0);
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                if (now > expires)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var response = JsonSerializer.Serialize(ApiResponseFactory.Unauthorized("Expired!"));
                    await context.Response.WriteAsync(response);
                    //await context.Response.WriteAsync("Expired");
                    return;
                }
                var isCandidateClaim = context.User.FindFirst("iscandidate")?.Value;
                bool isCandidateToken = bool.TryParse(isCandidateClaim, out bool parsed) && parsed;
                // Nếu Token là ứng viên VÀ Hệ thống đang cấu hình chặn ứng viên (IsCandidate = true)
                if (isCandidateToken)
                {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var response = JsonSerializer.Serialize(ApiResponseFactory.Unauthorized("Bạn không có quyền!"));
                        await context.Response.WriteAsync(response);
                        return;    
                }
                // Check là admin không
                var isAdminClaim = context.User.FindFirst("isadmin")?.Value;
                if (!string.IsNullOrEmpty(isAdminClaim) && bool.TryParse(isAdminClaim, out bool isAdmin) && isAdmin)
                {
                    await _next(context);
                    return;
                }

                // Check có mã quyền không
                if (permissionAttributes != null && permissionAttributes.Count > 0)
                {
                    foreach (var attr in permissionAttributes)
                    {
                        var hasPermission = await permissionService.HasPermissionAsync(userId, attr.permission);
                        if (!hasPermission)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "text/plain; charset=utf-8";
                            var response = JsonSerializer.Serialize(ApiResponseFactory.Unauthorized("Bạn không có quyền!"));
                            await context.Response.WriteAsync(response);
                            return;
                        }
                    }
                }
            }

            // Cần đảm bảo luôn gọi _next(context) ở bước cuối cùng của Middleware
            // Nếu không yêu cầu Authorize hoặc đã pass qua tất cả các check ở trên
            await _next(context);
        }
    }
}
using RERPAPI.Attributes;
using RERPAPI.IRepo;
using System.Security.Claims;

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
            var extractedApiKey = context.Request.Headers.TryGetValue("x-api-key", out var apiKey);
            if (extractedApiKey)
            {
                if (apiKey == _apiKey)
                {
                    context.Items["AuthType"] = "ApiKey";
                    await _next(context);
                    return;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Key không đúng");
                    return;
                }
            }
            else
            {
                var endpoint = context.GetEndpoint();

                var permissionAttributes = endpoint?.Metadata.GetOrderedMetadata<RequiresPermissionAttribute>();

                if (permissionAttributes != null && permissionAttributes.Count > 0)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }

                    permissionService.Claims = new Dictionary<string, string>();

                    foreach (var attr in permissionAttributes)
                    {
                        var hasPermission = await permissionService.HasPermissionAsync(userId, attr.permission);

                        if (!hasPermission)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync("Access Denied");
                            return;
                        }
                    }

                    await _next(context);

                }

            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }
}

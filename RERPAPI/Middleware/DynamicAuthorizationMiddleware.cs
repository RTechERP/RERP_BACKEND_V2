using RERPAPI.Attributes;
using RERPAPI.IRepo;
using System.Security.Claims;

namespace RERPAPI.Middleware
{
    public class DynamicAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public DynamicAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserPermissionService permissionService)
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

                permissionService.Claims = new Dictionary<string, string>()
                {
                    {"ID","1181" },
                    {"FullName","Lê Thế Anh" },
                    {"LoginName","ltanh" },
                };

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
            }

            await _next(context);
        }
    }
}
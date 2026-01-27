using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RERPAPI.IRepo;
using RERPAPI.Model.Context;

namespace RERPAPI.Repo
{
    public class UserPermissionService : IUserPermissionService
    {
        protected RTCContext _dbContext { get; set; }
        public Dictionary<string, string> Claims { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserPermissionService(RTCContext db, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            if (!int.TryParse(userId, out var id)) return false;

            var permissions = permission.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); //NTA B update 051125

            foreach (var perm in permissions) //NTA B update 051125
            {
                var hasPerm = await _dbContext.vUserGroupLinks.AnyAsync(p => p.UserID == id && p.Code == perm);
                if (hasPerm) return true;
            }

            //var isPermission = await _dbContext.vUserGroupLinks.AnyAsync(p => p.UserID == id && p.Code == permission);
            return false;
        }

        //public async Task<bool> HasPermissionAsync(string userId, string permission)
        //{
        //    if (!int.TryParse(userId, out var id)) return false;

        //    var permissions = permission
        //        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        //    return await _dbContext.vUserGroupLinks
        //        .AnyAsync(p => p.UserID == id && permissions.Contains(p.Code));
        //}


        public Dictionary<string, string> GetClaims()
        {
            //CurrentUser currentUser = new CurrentUser();

            var claims = _httpContextAccessor.HttpContext?.User?.Claims.ToDictionary(x => x.Type, x => x.Value);
            return claims;

            //var props = typeof(CurrentUser).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            //foreach (var prop in props)
            //{
            //    if (!prop.CanWrite) continue;

            //    var value = claims.TryGetValue(prop.Name.ToLower(), out var rawValuea);

            //    if (claims.TryGetValue(prop.Name.ToLower(), out var rawValue))
            //    {
            //        try
            //        {
            //            object? parsedValue = prop.PropertyType switch
            //            {
            //                Type t when t == typeof(string) => rawValue,
            //                Type t when t == typeof(int) || t == typeof(int?) => int.TryParse(rawValue, out var i) ? i : 0,
            //                Type t when t == typeof(bool) || t == typeof(bool?) => int.TryParse(rawValue, out var b) ? b : false,
            //                Type t when t == typeof(DateTime) || t == typeof(DateTime?) => int.TryParse(rawValue, out var d) ? d : null,
            //                _ => null
            //            };

            //            if (parsedValue != null) prop.SetValue(currentUser, parsedValue);
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception($"{prop.Name}\r\n{ex.Message}\r\n{ex.ToString()}");
            //        }
            //    }
            //}

            //return currentUser;
        }
    }
}
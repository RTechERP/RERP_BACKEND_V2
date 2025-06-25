using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.IRepo
{
    public interface IUserPermissionService
    {
        Task<bool> HasPermissionAsync(string userId, string permission);
        Dictionary<string, string> GetClaims();
    }
}

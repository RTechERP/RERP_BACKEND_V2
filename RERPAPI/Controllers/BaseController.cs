using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;

namespace RERPAPI.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //protected CurrentUser _currentUser;

        //    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //_currentUser = ObjectMapper.GetCurrentUser(claims);
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    base.OnActionExecuting(context);

        //    if (User?.Identity?.IsAuthenticated == true)
        //    {
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        _currentUser = ObjectMapper.GetCurrentUser(claims);
        //    }
        //}
    }
}

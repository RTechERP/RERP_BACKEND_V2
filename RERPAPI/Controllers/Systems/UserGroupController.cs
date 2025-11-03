using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity.Systems;

namespace RERPAPI.Controllers.Systems
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {

        UserGroupRepo _userGroupRepo = new UserGroupRepo();

        [HttpGet("get-user-group")]
        public IActionResult UserGroupByFormAndFunctionCode(string functionCode)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroupByFormAndFunctionCode", new string[] { "@FormAndFunctionCode" }, new object[] { functionCode });

                var data = new
                {
                    usergroup = SQLHelper<object>.GetListData(datas, 0),
                    codes = SQLHelper<object>.GetListData(datas, 1)[0].Codes
                };
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

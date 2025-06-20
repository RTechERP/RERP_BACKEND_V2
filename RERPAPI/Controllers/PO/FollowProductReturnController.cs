using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowProductReturnController : ControllerBase
    {
        UserRepo _userRepo = new UserRepo();
        // GET: api/<FollowProductReturnController>
        [HttpGet("get-data")]
        public IActionResult LoadData( int customerId, int userId, int groupSaleId, DateTime dateStart, DateTime dateEnd, string keywords = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHProductReturn",
                    new string[] { "@FilterText", "@CustomerID", "@UserID", "@Group", "@StartDate", "@EndDate" },
                    new object[] { keywords, customerId, userId, groupSaleId, dateStart, dateEnd });
                {
                    return Ok(new
                    {
                        status = 1,
                        data = SQLHelper<dynamic>.GetListData(list, 0)
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-users")]
        public IActionResult GetUser()
        {
            try
            {
                List<User> list = _userRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}

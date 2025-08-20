using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserRepo _userRepo;

    public UsersController(UserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpGet("cbb-user")]
    public IActionResult getDataCbbUser()
    {
        try
        {
            var result = SQLHelper<object>.ProcedureToList(
                "spGetUsersHistoryProductRTC", new string[] { "@UsersID" },
                new object[] { 0 }
            );

            return Ok(new
            {
                status = 1,
                data = SQLHelper<object>.GetListData(result, 0)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { status = 0, ex.Message });
        }
    }

    [HttpGet("cbb-sender")]
    public IActionResult getDataCbbSender()
    {
        try
        {
            var result = _userRepo.GetAll();
            return Ok(new { status = 1, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { status = 0, ex.Message });
        }
    }
}

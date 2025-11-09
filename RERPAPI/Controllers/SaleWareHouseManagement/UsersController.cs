using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using System.Text.RegularExpressions;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
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
        public IActionResult getDataCbbUser() {
            try
            {           
               var result = SQLHelper<object>.ProcedureToList(
                                    "spGetUsersHistoryProductRTC", new string[] { "@UsersID" },
                                 new object[] { 0}
                                );
                List<dynamic> rs = result[0];
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("cbb-sender")]
        public IActionResult getDataCbbSender() {
            try
            {
                List<User> result = _userRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }


        }

    }
}

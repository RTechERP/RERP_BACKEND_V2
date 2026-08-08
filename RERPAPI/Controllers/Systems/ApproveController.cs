using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.Systems
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApproveController : ControllerBase
    {
        [HttpPost("get-approve-by-approve-tp")]
   
        public async Task<ActionResult> GetApproveByApproveTP([FromBody] ApproveByApproveTPRequestParam request)

        {
            //spGetApprovedByApprovedTP_EarlyLate: đi muộn về sớm
            //spGetApprovedByApprovedTP_EmployeeBussiness:Công tác
            //spGetApprovedByApprovedTP_NightShift:làm đêm
            //spGetApprovedByApprovedTP_Nofinger:Quên chấm công
            //spGetApprovedByApprovedTP_OnLeave: Đăng ký nghỉ
            //spGetApprovedByApprovedTP_Overtime:Làm thêm 
            //spGetApprovedByApprovedTP_VehicleBooking: Đặt xe
            //spGetApprovedByApprovedTP_WFH: WFH
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isBGD = currentUser.DepartmentID == 1 && currentUser.EmployeeID != 54;
                if (isBGD == true)
                {
                    request.IDApprovedTP = 0;
                }
                request.DateStart = request.DateStart.Value.ToLocalTime().Date;
                request.DateEnd = request.DateEnd.Value.ToLocalTime().Date.AddDays(+1).AddSeconds(-1);
                var param = new
                {
                    FilterText = request.FilterText,
                    DateStart = request.DateStart,
                    DateEnd = request.DateEnd,
                    IDApprovedTP = request.IDApprovedTP,
                    Status = request.Status,
                    DeleteFlag = request.DeleteFlag,
                    EmployeeID = request.EmployeeID,
                    StatusHR = request.StatusHR,
                    StatusBGD = request.StatusBGD,
                    IsBGD = isBGD,
                    UserTeamID = request.UserTeamID,
                    SeniorID = request.SeniorID,
                    StatusSenior = request.StatusSenior
                };
                string procedureName = request.TType switch
                {
                    1 => "spGetApprovedByApprovedTP_OnLeave",
                    2 => "spGetApprovedByApprovedTP_EarlyLate",
                    3 => "spGetApprovedByApprovedTP_Overtime",
                    4 => "spGetApprovedByApprovedTP_EmployeeBussiness",
                    5 => "spGetApprovedByApprovedTP_WFH",
                    6 => "spGetApprovedByApprovedTP_Nofinger",
                    8 => "spGetApprovedByApprovedTP_NightShift",
                    9 => "spGetApprovedByApprovedTP_VehicleBooking",
                    _ => "spGetApprovedByApprovedTP_New"
                };

                var data = await SqlDapper<object>.ProcedureToListAsync(procedureName, param);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

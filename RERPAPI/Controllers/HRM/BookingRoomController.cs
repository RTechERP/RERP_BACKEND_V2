using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingRoomController : ControllerBase
    {
         BookingRoomRepo _bookingRoomRepo;

        public BookingRoomController(BookingRoomRepo bookingRoomRepo)
        {
            _bookingRoomRepo = bookingRoomRepo;
        }

        [HttpPost("get-booking-room")]
        public IActionResult GetBookingRoom(DateTime DateStart, DateTime DateEnd, bool IsShowWeb)
        {
            try
            {
                string procedureName = "spGetBookingRoom";
                string[] paramNames = new string[] { "@StartDate", "@EndDate", "@IsShowWeb" };
                object[] paramValues = new object[] { DateStart, DateEnd,1 };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var room1 = SQLHelper<object>.GetListData(data, 0);
                var room2 = SQLHelper<object>.GetListData(data, 1);
                var room3 = SQLHelper<object>.GetListData(data, 2);
                return Ok(ApiResponseFactory.Success(new {room1,room2,room3}, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] BookingRoom bookingRoom)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
              
                var validate = _bookingRoomRepo.Validate(bookingRoom);
                if (validate.status == 0) return BadRequest(validate);
                if (bookingRoom.ID <= 0)
                {
                    var isRegister = _bookingRoomRepo.IsHaveRegisterTime(bookingRoom);
                    if (isRegister.status == 0) return BadRequest(isRegister);
                    await _bookingRoomRepo.CreateAsync(bookingRoom);
                }
                {
                    if(bookingRoom.IsApproved>0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Phòng đăng ký đã được duyệt, không thể sửa"));
                    }    
                    else if(bookingRoom.EmployeeID != currentUser.EmployeeID)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể sửa đăng ký của người khác!"));

                    }    
                      else await _bookingRoomRepo.UpdateAsync(bookingRoom);
                }
              
                return Ok(ApiResponseFactory.Success(bookingRoom, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("delete-booking-room")]
        public async Task<IActionResult> DeleteBookingRoom(int id)
        {
            try
            {
               if(id>0)
                {
                    BookingRoom br = new BookingRoom();
                    br.ID = id;
                    br.IsDeleted = true;
                  await  _bookingRoomRepo.UpdateAsync(br);
                }    
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-by-id")]
        public IActionResult GetById(int id)
        {
            try
            {
                var room = _bookingRoomRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(room, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

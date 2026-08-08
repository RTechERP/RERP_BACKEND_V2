using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class BookingRoomRepo : GenericRepo<BookingRoom>
    {
        public BookingRoomRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public APIResponse IsHaveRegisterTime(BookingRoom model)
        {
            try
            {
                if (!model.DateRegister.HasValue || !model.StartTime.HasValue || !model.EndTime.HasValue)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập đầy đủ thông tin thời gian!");
                }

                var startTime = model.StartTime.Value.TimeOfDay;
                var endTime = model.EndTime.Value.TimeOfDay;
                var date = model.DateRegister.Value.Date;

                bool isOverlap = GetAll().Any(p => p.ID != model.ID
                                                      && p.DateRegister.HasValue && p.DateRegister.Value.Date == date
                                                      && p.EmployeeID > 0
                                                      && p.IsDeleted != true
                                                      && p.MeetingRoomID == model.MeetingRoomID
                                                      && p.StartTime.HasValue && p.EndTime.HasValue
                                                      && p.StartTime.Value.TimeOfDay < endTime
                                                      && startTime < p.EndTime.Value.TimeOfDay);

                if (isOverlap)
                {
                    return ApiResponseFactory.Fail(null, $"Phòng trong khung giờ {model.StartTime.Value:HH:mm} - {model.EndTime.Value:HH:mm} đã có người đăng ký!");
                }
                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }

        public APIResponse Validate(BookingRoom model)
        {
            try
            {
                if (!model.DateRegister.HasValue || !model.StartTime.HasValue || !model.EndTime.HasValue)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập đầy đủ thông tin thời gian!");
                }

                if (model.DateRegister.Value.Date < DateTime.Now.Date)
                {
                    return ApiResponseFactory.Fail(null, "Bạn không được đăng ký trước ngày hiện tại!");
                }
                if (model.StartTime.Value.TimeOfDay >= model.EndTime.Value.TimeOfDay)
                {
                    return ApiResponseFactory.Fail(null, "Thời gian bắt đầu không được >= Thời gian kết thúc");
                }

                if ((model.DateRegister.Value.Date - DateTime.Now.Date).TotalDays > 31)
                {
                    return ApiResponseFactory.Fail(null, "Ngày đăng ký phải trong vòng 1 tháng!");
                }

                var startTime = model.StartTime.Value.TimeOfDay;
                var endTime = model.EndTime.Value.TimeOfDay;
                var date = model.DateRegister.Value.Date;

                bool isExist = GetAll().Any(p => p.ID != model.ID
                                                      && p.DateRegister.HasValue && p.DateRegister.Value.Date == date
                                                      && p.EmployeeID > 0
                                                      && p.IsDeleted != true
                                                      && p.MeetingRoomID == model.MeetingRoomID
                                                      && p.StartTime.HasValue && p.EndTime.HasValue
                                                      && p.StartTime.Value.TimeOfDay < endTime
                                                      && startTime < p.EndTime.Value.TimeOfDay);

                if (isExist)
                {
                    return ApiResponseFactory.Fail(null, "Phòng đã được đăng ký trong khung giờ này!");
                }
                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
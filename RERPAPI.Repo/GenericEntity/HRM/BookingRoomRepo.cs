using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class BookingRoomRepo:GenericRepo<BookingRoom>
    {
        public BookingRoomRepo(CurrentUser currentUser) : base(currentUser)
        {

        }
        public APIResponse IsHaveRegisterTime(BookingRoom model)
        {
            try
            {
                List<BookingRoom> list = GetAll().Where(p => p.ID != model.ID
                                                      && p.DateRegister.Value.Date == model.DateRegister.Value.Date
                                                      && p.EmployeeID > 0
                                                      &&p.IsDeleted!=true
                                                      && p.MeetingRoomID == model.MeetingRoomID
                                                      && ((
                                                             p.StartTime.Value.TimeOfDay <= model.StartTime.Value.TimeOfDay
                                                             && p.EndTime.Value.TimeOfDay > model.StartTime.Value.TimeOfDay//NxLuong update 04/10/25
                                                           )
                                                             || (p.StartTime.Value.TimeOfDay < model.EndTime.Value.TimeOfDay //NxLuong update 04/10/25
                                                                 && p.EndTime.Value.TimeOfDay >= model.EndTime.Value.TimeOfDay)
                                                         ))
                                              .ToList();
                if(list.Count>0)
                {
                    return ApiResponseFactory.Fail(null, $"Phòng từ {model.StartTime} - {model.EndTime} đã có người đăng ký!");
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
                if (model.DateRegister.Value.Date < DateTime.Now.Date)
                {
                    return ApiResponseFactory.Fail(null, "Bạn không được đăng ký trước ngày hiện tại!");
                }
                if (model.StartTime.Value.TimeOfDay > model.EndTime.Value.TimeOfDay)
                {
                    return ApiResponseFactory.Fail(null, "Thời gian bắt đầu không được > Thời gian kết thúc");
                }

                if ((model.DateRegister.Value - DateTime.Now.AddMonths(+1)).TotalDays > 0)
                {
                    return ApiResponseFactory.Fail(null, "Ngày đăng ký phải là <= 1 tháng!");
                }

                List<BookingRoom> list = GetAll().Where(p => p.ID != model.ID
                                                      && p.DateRegister.Value.Date == model.DateRegister.Value.Date
                                                      && p.EmployeeID > 0
                                                      && p.MeetingRoomID == model.MeetingRoomID
                                                      && ((
                                                             p.StartTime.Value.TimeOfDay <= model.StartTime.Value.TimeOfDay
                                                             && p.EndTime.Value.TimeOfDay > model.StartTime.Value.TimeOfDay//NxLuong update 04/10/25
                                                           )
                                                             || (p.StartTime.Value.TimeOfDay < model.EndTime.Value.TimeOfDay //NxLuong update 04/10/25
                                                                 && p.EndTime.Value.TimeOfDay >= model.EndTime.Value.TimeOfDay)
                                                         ))
                                              .ToList();
                if (list.Count > 0)
                {
                    return ApiResponseFactory.Success(list, "Phòng đã được đăng ký trong khung giờ này!");
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

using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleBookingManagementRepo : GenericRepo<VehicleBookingManagement>
    {
        private readonly EmployeeSendEmailRepo _sendEmailRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly CurrentUser _currentUser;
        public VehicleBookingManagementRepo(CurrentUser currentUser, EmployeeRepo employeeRepo, EmployeeSendEmailRepo employeeSendEmailRepo) : base(currentUser)
        {
            this._currentUser = currentUser;
            this._employeeRepo = employeeRepo;
            this._sendEmailRepo = employeeSendEmailRepo;
        }

        public void SendEmail( VehicleBookingManagementDTO vehicleBooking, int receiver, string subject)
        {
            //string status = isAdd ? "ĐĂNG KÝ" : "CẬP NHẬT";
            //string startTime = timeStart.ToString("HH:mm");
            //string endTime = timeEnd.ToString("HH:mm");

            //if (Config._environment == 0) return;

            Employee employee = _employeeRepo.GetByID(receiver);
            employee = employee ?? new Employee();
            if (employee.ID <= 0) return;

            EmployeeSendEmail e = new EmployeeSendEmail();

            //DataTable userTeam = LoadDataFromSP.GetDataTableSP("spGetUserTeamByUserID", new string[] { "@UserID" }, new object[] { employee.UserID });
            var data = SQLHelper<dynamic>.ProcedureToList("spGetUserTeamByUserID", new string[] { "@UserID" }, new object[] { employee.UserID });
            var userTeam = SQLHelper<dynamic>.GetListData(data, 0);

            int userTeamID = 0;
            string emailCongTy = employee.EmailCongTy;
            if (userTeam.Count > 0 && userTeam != null)
            {
                dynamic row = userTeam[0];
                userTeamID = row.ID == null ? 0 : Convert.ToInt32(row.ID);

                if (userTeamID == 10 && row.EmailCongTy != null)
                {
                    emailCongTy = TextUtils.ToString(row.EmailCongTy);
                }

            }

            e.Subject = $"{subject.ToUpper()} - {_currentUser.FullName.ToUpper()} - {DateTime.Now.ToString("dd/MM/yyyy")}";
            e.EmailTo = emailCongTy;


            string timeNeed = !vehicleBooking.TimeNeedPresent.HasValue ? "" : vehicleBooking.TimeNeedPresent.Value.ToString("dd/MM/yyyy HH:mm");
            string departureDate = !vehicleBooking.DepartureDate.HasValue ? "" : vehicleBooking.DepartureDate.Value.ToString("dd/MM/yyyy HH:mm");


            if (vehicleBooking.ApprovedTBP > 0)
            {
                e.Body = $@"<div> <p style=""font-weight: bold; color: red;"">[NO REPLY]</p> <p> Dear anh/chị {employee.FullName}</p ></div >
                            <div style = ""margin-top: 30px;"">
                            <p> Em xin phép anh / chị cho em đăng kí xe:</p>
                            <p> Hình thức: {vehicleBooking.CategoryText}</p>
                            <p> Điểm xuất phát: {vehicleBooking.DepartureAddress}</p>
                            <p> Thời gian xuất phát: {departureDate}</p>
                            <p> Điểm đến: {vehicleBooking.CompanyNameArrives} - {vehicleBooking.SpecificDestinationAddress}</p>
                            <p> Thời gian cần đến: {timeNeed}</p>
                            <p> Anh / chị duyệt giúp em với ạ.Em cảm ơn! </p>
                            </div>
                            <div style = ""margin-top: 30px;"">
                            <p> Thanks </p>
                            <p> {_currentUser.FullName}</p>
                        </div>";
            }
            else
            {

                e.Body = $@"<div> <p style=""font-weight: bold; color: red;"">[NO REPLY]</p> <p> Dear {employee.FullName}</p ></div >
                            <div style = ""margin-top: 30px;"">
                            <p> {_currentUser.FullName} đã đăng ký xe cho bạn:</p>
                            <p> Hình thức: {vehicleBooking.CategoryText}</p>
                            <p> Điểm xuất phát: {vehicleBooking.DepartureAddress}</p>
                            <p> Thời gian xuất phát: {departureDate}</p>
                            <p> Điểm đến: {vehicleBooking.CompanyNameArrives} -{vehicleBooking.SpecificDestinationAddress}</p>
                            <p> Thời gian cần đến: {timeNeed}</p>
                            <p> Nhớ theo dõi lịch xe để đi đúng giờ nhé!</p>
                            </div>
                            <div style = ""margin-top: 30px;"">
                            <p> Thanks </p>
                            <p> {_currentUser.FullName}</p>
                        </div>";
            }



            e.StatusSend = 1;
            e.EmployeeID = _currentUser.EmployeeID;
            e.Receiver = employee.ID;
            _sendEmailRepo.Create(e);
        }

        public bool IsProblem(VehicleBookingManagement vehicleBooking)
        {
            bool isProblem = false;
            try
            {
                if (!vehicleBooking.TimeNeedPresent.HasValue) return isProblem;
                DateTime dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 00);
                DateTime minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 30, 00);
                DateTime maxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00);

                DateTime timeNeedPresent = new DateTime(vehicleBooking.TimeNeedPresent.Value.Year, vehicleBooking.TimeNeedPresent.Value.Month, vehicleBooking.TimeNeedPresent.Value.Day, vehicleBooking.TimeNeedPresent.Value.Hour, vehicleBooking.TimeNeedPresent.Value.Minute, 00);
                if (vehicleBooking.ID > 0 && vehicleBooking.CreatedDate.HasValue)
                {
                    dateNow = new DateTime(vehicleBooking.CreatedDate.Value.Year, vehicleBooking.CreatedDate.Value.Month, vehicleBooking.CreatedDate.Value.Day, vehicleBooking.CreatedDate.Value.Hour, vehicleBooking.CreatedDate.Value.Minute, 00);
                }

                if (dateNow.Hour >= 20)
                {
                    isProblem = true;
                    return isProblem;
                }
                else if (timeNeedPresent.Date == dateNow.Date)
                {
                    isProblem = !(dateNow >= minDate && dateNow <= maxDate);
                }

                return isProblem;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

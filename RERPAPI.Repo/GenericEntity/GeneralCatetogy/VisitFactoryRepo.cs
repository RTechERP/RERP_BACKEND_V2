using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
	public class VisitFactoryRepo : GenericRepo<VisitFactory>
	{

        EmployeeRepo employeeRepo = new EmployeeRepo();
        VisitFactoryDetailRepo visitFactoryDetailRepo = new VisitFactoryDetailRepo();
        EmployeeSendEmailRepo sendEmailRepo = new EmployeeSendEmailRepo();
        public async Task SendEmail(VisitFactory visit)
		{
            Employee employee = employeeRepo.GetByID(visit.EmployeeID);
            EmployeeSendEmail sendEmail = new EmployeeSendEmail();
            sendEmail.Subject = $"{employee.FullName} - ĐĂNG KÝ THĂM NHÀ MÁY: {visit.DateVisit.ToString("dd/MM/yyyy")}".ToUpper();
            sendEmail.EmailTo = "nguyentuan.dang@rtc.edu.vn";
            sendEmail.EmailCC = "admin11@rtc.edu.vn";
            sendEmail.StatusSend = 1;
            sendEmail.DateSend = DateTime.Now;
            sendEmail.EmployeeID = visit.EmployeeID;
            sendEmail.Receiver = visit.EmployeeReceive;
            sendEmail.TableInfor = "VisitFactory";

            string tbody = "";

            var details = visitFactoryDetailRepo.GetAll(x => x.VisitFactoryID == visit.ID);
            foreach (var item in details)
            {
                tbody += $@"<tr>
                                <td>{item.FullName}</td>
                                <td>{item.Company}</td>
                                <td>{item.Position}</td>
                                <td>{item.Phone}</td>
                                <td>{item.Email}</td>
                            </tr>";
            }
            sendEmail.Body = $@"
                            <div style='font-family: Arial; font-size: 14px;'>
                                Dear <b> Nguyễn Tuấn Đăng</b>,<br/><br/>
                                Nhân viên <b>{employee.FullName}</b> đăng ký thăm nhà máy ngày {visit.DateVisit.ToString("dd/MM/yyyy")} từ {visit.DateStart.Value.ToString("HH:mm")} đến {visit.DateEnd.Value.ToString("HH:mm")}<br/><br/>
                                Danh sách người tham gia bao gồm:
                                <table border='1' cellspacing='0' cellpadding='5' style='border-collapse:collapse; width:100%;'>
                                    <thead style='background-color:#f2f2f2;'>
                                        <tr>
                                            <th>Họ tên</th>
                                            <th>Công ty/ Đơn vị</th>
                                            <th>Chức vụ</th>
                                            <th>Số điện thoại</th>
                                            <th>Email</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {tbody}
                                    </tbody>
                                </table>
                                <br/>
                                Ghi chú: {visit.Note}
                                <br/>
                                Trân trọng.
                            </div>";



            await sendEmailRepo.CreateAsync(sendEmail);
        }
	}
}



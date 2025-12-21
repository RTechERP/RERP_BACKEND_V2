using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.JobRequirements
{
    public class JobRequirementRepo : GenericRepo<JobRequirement>
    {
        private EmployeeRepo _employeeRepo;
        private JobRequirementDetailRepo _jobRequirementDetailRepo;
        private CurrentUser _currentUser;
        private EmployeeSendEmailRepo _employeeSendEmailRepo;
        public JobRequirementRepo(CurrentUser currentUser, JobRequirementDetailRepo jobRequirementDetailRepo, EmployeeRepo employeeRepo, EmployeeSendEmailRepo employeeSendEmailRepo) : base(currentUser)
        {
            _jobRequirementDetailRepo = jobRequirementDetailRepo;
            _currentUser = currentUser;
            _employeeRepo = employeeRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
        }
        public void SendMail(JobRequirement job)
        {
            //todo send mail
            if(job.ID<=0)
            {
                return;
            }
            EmployeeSendEmail sendEmail = new EmployeeSendEmail();
            var employeeTP = _employeeRepo.GetByID(job.ApprovedTBPID ?? 0);
            var detail = _jobRequirementDetailRepo.GetAll(x=>x.JobRequirementID == job.ID && x.IsDeleted != true).ToList();
            JobRequirementDetail contents = detail.Where(x => x.STT == 1).FirstOrDefault() ?? new JobRequirementDetail();
            JobRequirementDetail reason = detail.Where(x => x.STT == 3).FirstOrDefault() ?? new JobRequirementDetail();

            JobRequirementDetail deadline = detail.Where(x => x.STT == 7).FirstOrDefault() ?? new JobRequirementDetail();
            sendEmail.Subject = $"YÊU CẦU CÔNG VIỆC - {_currentUser.FullName.ToUpper()} - {DateTime.Now.ToString("dd/MM/yyyy")}";
            sendEmail.EmailTo = $"{employeeTP.EmailCongTy}";
            //sendEmail.EmailCC = $"hanhchinh@rtc.edu.vn";
            sendEmail.Body = $@"<div> <p style=""font-weight: bold; color: red;"">[NO REPLY]</p> <p> Dear anh/chị {employeeTP.FullName} </p ></div >
                       <div style = ""margin-top: 30px;"">
                        <p> Anh/chị cho em đăng ký phiếu yêu cầu công việc</p>
                        <p> Nội dung: {contents.Description}</p>
                        <p> Lý do: {reason.Description}</p>
                        <p> Thời gian cần hoàn thành: {deadline.Description}</p>
                        <p> Anh / chị duyệt giúp em với ạ.Em cảm ơn! </p>
                       </div>
                       <div style = ""margin-top: 30px;"">
                        <p> Thanks </p>
                        <p> {_currentUser.FullName}</p>
                       </div>";

            sendEmail.StatusSend = 1;
            sendEmail.EmployeeID = job.EmployeeID; //Người gửi
            sendEmail.Receiver = job.ApprovedTBPID; //Người nhận
            _employeeSendEmailRepo.Create(sendEmail);
        }
    }
}

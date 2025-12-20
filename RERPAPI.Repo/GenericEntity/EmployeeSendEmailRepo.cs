using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeSendEmailRepo : GenericRepo<EmployeeSendEmail>
    {
        EmployeeRepo employeeRepo;
        public EmployeeSendEmailRepo(CurrentUser currentUser, EmployeeRepo employeeRepo) : base(currentUser)
        {
            this.employeeRepo = employeeRepo;
        }
        public int SendMail(int employeeid, int receiver, string subject, string body, string cc)
        {

            try
            {
                EmployeeSendEmail sendEmail = new EmployeeSendEmail();
                Employee employee = employeeRepo.GetByID(receiver);
                Employee employeeSend = employeeRepo.GetByID(employeeid);

                List<string> listEmailCC = new List<string>();
                if (!string.IsNullOrEmpty(cc.Trim()))
                {
                    if (!listEmailCC.Contains(cc.Trim()))
                    {
                        listEmailCC.Add(cc.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(employeeSend.EmailCongTy.Trim()))
                {
                    if (!listEmailCC.Contains(employeeSend.EmailCongTy.Trim()))
                    {
                        listEmailCC.Add(employeeSend.EmailCongTy.Trim());
                    }
                }
                else
                {
                    if (!listEmailCC.Contains(employeeSend.EmailCaNhan.Trim()))
                    {
                        listEmailCC.Add(employeeSend.EmailCaNhan.Trim());
                    }
                }


                if (employee != null)
                {

                    string emailCC = string.Join(";", listEmailCC);

                    sendEmail.EmailTo = employee.EmailCongTy;
                    sendEmail.EmailCC = emailCC;
                    sendEmail.EmployeeID = employeeid;
                    sendEmail.Receiver = receiver;
                    sendEmail.Subject = subject;
                    sendEmail.Body = body;
                    sendEmail.StatusSend = 1;
                    if (Create(sendEmail) == 1)
                    {
                        return sendEmail.ID;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}

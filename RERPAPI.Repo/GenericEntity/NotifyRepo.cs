using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class NotifyRepo:GenericRepo<Notify>
    {
        public NotifyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public void AddNotify(string title, string text, int employeeID, int departmentID = 0)
        {
            try
            {
                Notify notify = new Notify();
                notify.Title = title;
                notify.Text = text;
                notify.EmployeeID = employeeID;
                notify.DepartmentID = departmentID;
                notify.NotifyStatus = 1;
                Create(notify);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

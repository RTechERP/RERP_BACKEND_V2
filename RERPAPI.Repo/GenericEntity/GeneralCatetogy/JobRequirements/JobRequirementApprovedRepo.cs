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
    public class JobRequirementApprovedRepo : GenericRepo<JobRequirementApproved>
    {
        DepartmentRepo _departmentRepo;
        CurrentUser _currentUser;
        EmployeeApproveRepo _employeeApproveRepo;
        public JobRequirementApprovedRepo(CurrentUser currentUser, DepartmentRepo departmentRepo, EmployeeApproveRepo employeeApproveRepo) : base(currentUser)
        {
            _departmentRepo = departmentRepo;
            _currentUser = currentUser;
            _employeeApproveRepo = employeeApproveRepo;
        }
        public async Task CreateJobRequirementApproved(int approvedTBPID, JobRequirement model)
        {
            string _typeHanhChinh = "2,3,4,5,6,7";
            string _typeHR = "1,4,5";
            string[] stepNames = new string[] { "", "Tạo phiếu yêu cầu công việc", "TBP xác nhận", "HR check yêu cầu", "TBP HR xác nhận", "BGĐ xác nhận" };

            var list = GetAll(x => x.JobRequirementID == model.ID).ToList();
            foreach (var item in list)
            {
                Delete(item.ID);
            }
            List<JobRequirementApproved> listLog = new List<JobRequirementApproved>();
            var headOfHR = _departmentRepo
            .GetAll(x => x.Code == "HR")
            .Select(x => x.HeadofDepartment)
            .FirstOrDefault() ?? 0;
            var headOfKT = _departmentRepo
           .GetAll(x => x.Code == "KT")
           .Select(x => x.HeadofDepartment)
           .FirstOrDefault() ?? 0;
            var typeHanhChinh = _typeHanhChinh.Split(',');
            var typeHR = _typeHR.Split(',');
            listLog.Add(new JobRequirementApproved() { JobRequirementID = model.ID, Step = 1, StepName = stepNames[1], DateApproved = DateTime.Now, IsApproved = 1, ApprovedID = _currentUser.EmployeeID, ApprovedActualID = _currentUser.EmployeeID, CreatedBy = _currentUser.LoginName, CreatedDate = DateTime.Now, UpdatedBy = _currentUser.LoginName, UpdatedDate = DateTime.Now });
            listLog.Add(new JobRequirementApproved() { JobRequirementID = model.ID, Step = 2, StepName = stepNames[2], DateApproved = null, IsApproved = 0, ApprovedID = approvedTBPID, ApprovedActualID = 0, CreatedBy = _currentUser.LoginName, CreatedDate = DateTime.Now, UpdatedBy = _currentUser.LoginName, UpdatedDate = DateTime.Now });
            listLog.Add(new JobRequirementApproved() { JobRequirementID = model.ID, Step = 3, StepName = stepNames[3], DateApproved = null, IsApproved = 0, ApprovedID = headOfHR, ApprovedActualID = 0, CreatedBy = _currentUser.LoginName, CreatedDate = DateTime.Now, UpdatedBy = _currentUser.LoginName, UpdatedDate = DateTime.Now });
            listLog.Add(new JobRequirementApproved() { JobRequirementID = model.ID, Step = 4, StepName = stepNames[4], DateApproved = null, IsApproved = 0, ApprovedID = headOfHR, ApprovedActualID = 0, CreatedBy = _currentUser.LoginName, CreatedDate = DateTime.Now, UpdatedBy = _currentUser.LoginName, UpdatedDate = DateTime.Now });
            listLog.Add(new JobRequirementApproved() { JobRequirementID = model.ID, Step = 5, StepName = stepNames[5], DateApproved = null, IsApproved = 0, ApprovedID = 1, ApprovedActualID = 0, CreatedBy = _currentUser.LoginName, CreatedDate = DateTime.Now, UpdatedBy = _currentUser.LoginName, UpdatedDate = DateTime.Now });
            var tbpApprove = _employeeApproveRepo.GetAll(x => x.EmployeeID == _currentUser.EmployeeID && x.Type == 3 && x.IsDeleted != true).FirstOrDefault();
            if (tbpApprove != null)
            {
                var logTBP = listLog.FirstOrDefault(x => x.Step == 2);
                if (logTBP != null)
                {
                    logTBP.IsApproved = 1;
                    logTBP.ApprovedID = _currentUser.EmployeeID;
                    logTBP.ApprovedActualID = _currentUser.EmployeeID;
                    logTBP.DateApproved = DateTime.Now;
                }
            }
            foreach (JobRequirementApproved item in listLog)
            {
                await CreateAsync(item);
            }
            //return null;
        }
      

    }
}

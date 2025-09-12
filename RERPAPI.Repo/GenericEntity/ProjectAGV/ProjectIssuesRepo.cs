using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectIssuesRepo : GenericRepo<ProjectIssues>
    {
        private readonly EmployeeRepo _employeeDocumentRepo = new EmployeeRepo();
        public bool Validate(ProjectIssueDTO item, out string message)
        {
            message = "";

            if (!item.Status.HasValue)
            {
                message = "Vui lòng chọn trạng thái";
                return false;
            }
            if (string.IsNullOrEmpty(item.Title))
            {
                message = "Vui lòng nhập tiêu đề";
                return false;
            }
         
            if (item.Impact == null)
            {
                message = "Vui lòng nhập sự ảnh hưởng";
                return false;

            }

            if (item.Probability == null)
            {
                message = "Vui lòng nhập khả năng";
                return false;

            }
            if (item.EmployeeCode != null && item.EmployeeCode.Trim().Length > 0)
            {
                var employeeByEmployeeCode = _employeeDocumentRepo.GetAll().FirstOrDefault(x => x.Code.ToLower().Equals(item.EmployeeCode));
                if (employeeByEmployeeCode == null)
                {
                    message = "Vui lòng nhập mã nhân viên chính xác";
                    return false;
                }
            }
            else
            {
                message = "Vui lòng nhập mã nhân viên chính xác";
                return false;
            }
            return true;
        }
        public ProjectIssues MapFromProjectIssueDTOToFromProject(ProjectIssueDTO projectIssueDTO)
        {
            var projectIssues = new ProjectIssues();
            projectIssues.ID = projectIssueDTO.ID;
            projectIssues.Status = projectIssueDTO.Status;
            projectIssues.Title = projectIssueDTO.Title;
            projectIssues.ProjectID = projectIssueDTO.ProjectID;
            projectIssues.Description = projectIssueDTO.Description;
            projectIssues.Impact = projectIssueDTO.Impact;
            projectIssues.Status = projectIssueDTO.Status;
            projectIssues.Solution = projectIssueDTO.Solution;
            projectIssues.Description = projectIssueDTO.Description;
            projectIssues.MitigationPlan = projectIssueDTO.MitigationPlan;
            projectIssues.FilePath = projectIssueDTO.FilePath;
            projectIssues.CreatedDate = projectIssueDTO.CreatedDate;
            projectIssues.UpdatedDate = projectIssueDTO.UpdatedDate;
            projectIssues.Description = projectIssueDTO.Description;
            projectIssues.Description = projectIssueDTO.Description;
            projectIssues.Description = projectIssueDTO.Description;

            if (projectIssueDTO.EmployeeCode != null && projectIssueDTO.EmployeeCode.Trim().Length > 0)
            {
                var employeeByEmployeeCode = _employeeDocumentRepo.GetAll().FirstOrDefault(x => x.Code.ToLower().Equals(projectIssueDTO.EmployeeCode));
                if (employeeByEmployeeCode != null)
                {
                    projectIssues.EmployeeID = employeeByEmployeeCode.ID;
                }
            }
            return projectIssues;
        }
    }
}

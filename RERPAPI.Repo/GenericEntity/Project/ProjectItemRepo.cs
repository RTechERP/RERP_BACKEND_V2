using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectItemRepo : GenericRepo<ProjectItem>
    {
        ProjectRepo projectRepo = new ProjectRepo();
        public string GenerateProjectItemCode(int projectId)
        {
            try
            {
                var projectItems = GetAll(x => x.ProjectID == projectId);
                var project = projectRepo.GetByID(projectId);
                if (project.ID <= 0)
                {
                    throw new Exception($"Không có Project nào có ID là :{projectId}");
                }

                string newCode = $"{project.ProjectCode}_{projectItems.Count + 1}";
                return newCode;
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex.Message}\r\n{ex.ToString()}");
            }

        }
       public bool Validate(ProjectItem item, out string message)
        {
            message = "";
            if (item.ID < 0 || item.CreatedDate.HasValue)
            {
                if (item.TypeProjectItem <= 0)
                {
                    message = "Vui lòng chọn kiểu dự án";
                    return false;
                }
                if (item.EmployeeIDRequest <= 0)
                {
                    message = "Vui lòng chọn người giao việc";
                    return false;
                }
            }
            if (string.IsNullOrEmpty(item.Mission))
            {
                message = "Vui lòng nhập công việc";
                return false;
            }
            if (item.UserID <= 0)
            {
                message = "Vui lòng nhập người phụ trách";
                return false;
            }
            if (!item.PlanStartDate.HasValue)
            {
                message = "Vui lòng nhập ngày bắt đầu";
                return false;
            }
            if (!item.PlanEndDate.HasValue)
            {
                message = "Vui lòng nhập ngày kết thúc";
                return false;
            }
            if (item.Status == 2)
            {
                if (!item.ActualStartDate.HasValue)
                {
                    message = "Vui lòng nhập ngày bắt đầu thực tế";
                    return false;
                }
                if (!item.ActualEndDate.HasValue)
                {
                    message = "Vui lòng nhập ngày kết thúc thực tế";
                    return false;
                }
                if (item.ActualStartDate > item.ActualEndDate)
                {
                    message = "Ngày kết thúc phải lớn hơn ngày thực tế";
                    return false;
                }
            }
            return true;
        }
      

    }

}

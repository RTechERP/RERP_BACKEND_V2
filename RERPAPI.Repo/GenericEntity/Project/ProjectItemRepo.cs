using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.ProjectAGV;
using RERPAPI.Model.Entities;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectItemRepo : GenericRepo<ProjectItem>
    {
        ProjectRepo _projectRepo;

        public ProjectItemRepo(CurrentUser currentUser, ProjectRepo projectRepo) : base(currentUser)
        {
            _projectRepo = projectRepo;
        }

        public string GenerateProjectItemCode(int projectId)
        {
            try
            {
                var projectItem = GetAll(x => x.ProjectID == projectId);
                var project = _projectRepo.GetByID(projectId);
                if (project.ID <= 0)
                {
                    throw new Exception($"Không có Project nào có ID là :{projectId}");
                }

                string newCode = $"{project.ProjectCode}_{projectItem.Count + 1}";
                return newCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}\r\n{ex.ToString()}");
            }
        }
        public string GetMaxSTT(int? projectID)
        {
            if (projectID is null) return "1";
            var max = GetAll(x => x.ProjectID == projectID)
                .Select(x => x.STT)
                .AsEnumerable()
                .Select(s =>
                {
                    if (string.IsNullOrWhiteSpace(s)) return (int?)null;
                    return int.TryParse(s.Trim(), out var n) ? n : (int?)null;
                })
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .DefaultIfEmpty(0)
                .Max();
            return (max + 1).ToString();
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
        public bool ValidateAGV(ProjectItem item, out string message)
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

            if (item.ProjectID <= 0 || item.ProjectID == null)
            {
                message = "Vui lòng chọn dự án";
                return false;
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
        public bool ValidateDTO(ProjectItemDTO item, out string message)
        {
            message = "";
            if (item.ID < 0 || item.CreatedDate.HasValue)
            {
                if (item.TypeProjectItem <= 0)
                {
                    message = "Vui lòng chọn kiểu dự án";
                    return false;
                }

            }

            if (item.ProjectID <= 0 || item.ProjectID == null)
            {
                message = "Vui lòng chọn dự án";
                return false;
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

                if (!item.ActualEndDate.HasValue)
                {
                    message = "Vui lòng nhập ngày kết thúc thực tế";
                    return false;
                }
                if (item.PlanStartDate > item.PlanEndDate)
                {
                    message = "Ngày kết thúc phải lớn hơn ngày thực tế";
                    return false;
                }
            }
            return true;
        }
        //public int  GetMaxSTT(int projectId)
        //{
        //    try
        //    {
        //        var projectItem = GetAll(x => x.ProjectID == projectId);



        //        int maxSTT  = projectItem.Max(x=>x.STT);
        //        return maxSTT;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Lỗi:" + ex.Message);
        //    }
        //}

    }

}

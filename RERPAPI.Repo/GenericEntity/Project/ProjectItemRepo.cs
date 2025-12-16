using Azure.Core;
using RERPAPI.Model.Common;
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
                    message = $"Hạng mục [{item.Code}]: Vui lòng chọn kiểu dự án";
                    return false;
                }
                if (item.EmployeeIDRequest <= 0)
                {
                    message = $"Hạng mục [{item.Code}]: Vui lòng chọn người giao việc";
                    return false;
                }
            }
            if (string.IsNullOrEmpty(item.Mission))
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập công việc";
                return false;
            }
            if (item.UserID <= 0)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập người phụ trách";
                return false;
            }
            if (!item.PlanStartDate.HasValue)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày bắt đầu";
                return false;
            }
            if (!item.PlanEndDate.HasValue)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày kết thúc";
                return false;
            }
            if (item.Status == 2)
            {
                if (!item.ActualStartDate.HasValue)
                {
                    message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày bắt đầu thực tế";
                    return false;
                }
                if (!item.ActualEndDate.HasValue)
                {
                    message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày kết thúc thực tế";
                    return false;
                }
                if (item.ActualStartDate > item.ActualEndDate)
                {
                    message = $"Hạng mục [{item.Code}]: Ngày kết thúc phải lớn hơn ngày thực tế";
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
                    message = $"Hạng mục [{item.Code}]: Vui lòng chọn kiểu dự án";
                    return false;
                }
                if (item.EmployeeIDRequest <= 0)
                {
                    message = $"Hạng mục [{item.Code}]: Vui lòng chọn người giao việc";
                    return false;
                }
            }

            if (item.ProjectID <= 0 || item.ProjectID == null)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng chọn dự án";
                return false;
            }
            if (string.IsNullOrEmpty(item.Mission))
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập công việc";
                return false;
            }
            if (item.UserID <= 0)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập người phụ trách";
                return false;
            }
            if (!item.PlanStartDate.HasValue)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày bắt đầu";
                return false;
            }
            if (!item.PlanEndDate.HasValue)
            {
                message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày kết thúc";
                return false;
            }
            if (item.Status == 2)
            {

                if (!item.ActualEndDate.HasValue)
                {
                    message = $"Hạng mục [{item.Code}]: Vui lòng nhập ngày kết thúc thực tế";
                    return false;
                }
                if (item.ActualStartDate > item.ActualEndDate)
                {
                    message = $"Hạng mục [{item.Code}]: Ngày kết thúc phải lớn hơn ngày thực tế";
                    return false;
                }
            }
            return true;
        }
        public void CalculateDays(ProjectItem item)
        {
            item.TotalDayActual = item.ActualStartDate.HasValue && item.ActualEndDate.HasValue
                ? (decimal)(item.ActualEndDate.Value - item.ActualStartDate.Value).TotalDays
                : 0;
        }
        public bool CanDelete(ProjectItem item, CurrentUser user)
        {
            if (user.IsAdmin) return true;
            if (item.IsApproved > 0) return false;

            bool isTBP = user.EmployeeID == 54; // (user.EmployeeID == 54 || user.EmployeeID == user.HeadofDepartment);
            bool isPBP = user.PositionCode == "CV57" || user.PositionCode == "CV28";

            return isTBP || isPBP;
        }
        public bool CanEdit(ProjectItem item, CurrentUser user)
        {
            var check = SQLHelper<dynamic>.ProcedureToList("spGetProjectEmployeePermisstion",
                 new[] { "@ProjectID", "@EmployeeID" },
            new object[] { item.ProjectID, user.EmployeeID });
            return item.CreatedBy?.Equals(user.LoginName, StringComparison.OrdinalIgnoreCase) == true
            || item.UserID == user.ID
                || check.Count > 0
                || user.IsAdmin == true
                || item.EmployeeIDRequest == user.EmployeeID;
        }
        public async Task UpdatePercent(int projectId)
        {
            var items = GetAll(x => x.ProjectID == projectId && x.IsDeleted != true).ToList();
            var total = items.Sum(x => x.TotalDayPlan);

            foreach (var item in items)
            {
                item.PercentItem = total > 0 ? (item.TotalDayPlan / total) * 100 : 0;
                await UpdateAsync(item);
            }
        }
        public async Task UpdateLate(int projectId)
        {
            var items = GetAll(x => x.ProjectID == projectId && x.IsDeleted != true).ToList();
            var now = DateTime.Now.Date;

            foreach (var item in items)
            {
                int late = 0;

                if (item.ActualStartDate.HasValue && !item.ActualEndDate.HasValue && item.PlanEndDate.HasValue)
                {
                    if ((item.ActualStartDate.Value.Date - item.PlanEndDate.Value.Date).TotalDays > 0
                        || (now - item.PlanEndDate.Value.Date).TotalDays > 0)
                        late = 2;
                }
                else if (item.ActualStartDate.HasValue && item.ActualEndDate.HasValue && item.PlanEndDate.HasValue)
                {
                    if ((item.ActualEndDate.Value.Date - item.PlanEndDate.Value.Date).TotalDays > 0)
                        late = 1;
                }
                else if (!item.ActualStartDate.HasValue && !item.ActualEndDate.HasValue && item.PlanEndDate.HasValue)
                {
                    if ((now - item.PlanEndDate.Value.Date).TotalDays > 0)
                        late = 2;
                }
                else if (item.PlanStartDate.HasValue && !item.PlanEndDate.HasValue
                    && !item.ActualStartDate.HasValue && !item.ActualEndDate.HasValue)
                {
                    if ((now - item.PlanStartDate.Value.Date).TotalDays > 0)
                        late = 2;
                }

                item.ItemLate = late;
                await UpdateAsync(item);
            }
        }
       /* public async Task<string> GetMaxCodeAsync(int projectId, string projectCode)
        {
            var count = await _context.ProjectItems
                .CountAsync(x => x.ProjectID == projectId);
            return $"{projectCode}_{count + 1}";
        }*/

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

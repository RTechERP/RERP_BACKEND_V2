using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectItemProblemRepo : GenericRepo<ProjectItemProblem>
    {
        ProjectItemRepo _projectItemRepo;
        public ProjectItemProblemRepo(CurrentUser currentUser, ProjectItemRepo projectItemRepo) : base(currentUser)
        {
            _projectItemRepo = projectItemRepo;
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
        public string UpdateReasonLateProjectItem(int projectItemID, string loginName, int isUpdateProjectItem)
        {
            string reasonLate = "";
            var listProblem = GetAll().Where(x => x.ProjectItemID == projectItemID).OrderByDescending(x => x.CreatedDate).ToList();
            foreach (ProjectItemProblem problem in listProblem)
            {
                reasonLate += $"{problem.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm:ss")} - {problem.ContentProblem}\n";
            }
            if (isUpdateProjectItem == 1)
            {
                ProjectItem item = _projectItemRepo.GetByID(projectItemID);
                item.ReasonLate = reasonLate;
                item.UpdatedDate = DateTime.Now;
                item.UpdatedBy = loginName;
                _projectItemRepo.Update(item);
            }

            return reasonLate;
        }
    }
}

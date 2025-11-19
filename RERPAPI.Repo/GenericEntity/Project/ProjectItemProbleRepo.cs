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
        public ProjectItemProblemRepo(CurrentUser currentUser) : base(currentUser)
        {

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
    }
}

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
        public string GenerateProjectItemCode(int projectId)
        {
            var listItem = table.Where(x => x.ProjectID == projectId).ToList();
            var project = db.Projects.FirstOrDefault(p => p.ID == projectId);
            if (project == null)
            {
                throw new Exception($"Không tìm thấy Project với ID = {projectId}");
            }
            string newCode = $"{project.ProjectCode}_{listItem.Count + 1}";
            return newCode;
        }
    }

}

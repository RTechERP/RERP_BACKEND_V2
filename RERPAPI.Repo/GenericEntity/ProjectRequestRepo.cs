using Azure.Core;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectRequestRepo : GenericRepo<ProjectRequest>
    {
        public string GetRequestCode(int projectId)
        {
            var requestByProject = GetAll(x => x.ProjectID == projectId).ToList();
            int stt = requestByProject.Count > 0 ? requestByProject.Max(x => (x.STT ?? 0)) + 1 : 1;
            return $"PRQ{requestByProject.Count + 1}";
        }

    }
}
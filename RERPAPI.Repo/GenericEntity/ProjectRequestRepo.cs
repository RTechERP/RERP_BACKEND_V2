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
        public bool Validate(ProjectRequest request, out string message)
        {
            message = string.Empty;
            if (request.ProjectID <= 0)
            {
                message = "Vui lòng chọn Dự án";
                return false;
            }
            if (string.IsNullOrEmpty(request.CodeRequest))
            {
                message = "Vui lòng chọn Mã yêu cầu";
                return false;
            }
            var oldRequest = GetAll(x => x.ID != request.ID && x.CodeRequest == request.CodeRequest && x.ProjectID == request.ProjectID && x.IsDeleted == false);
            if (oldRequest.Count > 0)
            {
                message = $"Yêu cầu công việc có mã[{request.CodeRequest}] đã tồn tại!";
                return false;
            }
            return true;
        }
    }
}
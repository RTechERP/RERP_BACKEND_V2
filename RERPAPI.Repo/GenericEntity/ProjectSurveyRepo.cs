using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectSurveyRepo : GenericRepo<ProjectSurvey>
    {
        private readonly ProjectSurveyDetailRepo _projectSurveyDetail;
        public ProjectSurveyRepo(CurrentUser currentUser, ProjectSurveyDetailRepo projectSurveyDetail) : base(currentUser)
        {
            _projectSurveyDetail = projectSurveyDetail ;
        }
        public bool ValidateDeleted(ProjectSurvey projectSurvey, CurrentUser currentUser, out string message)
        {
            message = string.Empty;
            List<ProjectSurveyDetail> rs = _projectSurveyDetail.GetAll(x => x.ProjectSurveyID == projectSurvey.ID);
            if (currentUser.EmployeeID != projectSurvey.EmployeeID && !currentUser.IsAdmin)
            {
                message = "Bạn không thể xóa yêu cầu khảo sát của người khác";
                return false;
            }
            bool isConfirm = rs.Any(x => x.Status == 1);
            if(isConfirm && !currentUser.IsAdmin)
            {
                message = "Bạn không thể xóa yêu cầu khảo sát vì lwader Kỹ thuật đã xác nhận";
                return false;
            }
            return true;
        }
        }
    }

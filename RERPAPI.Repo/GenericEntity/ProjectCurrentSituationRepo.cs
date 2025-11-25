using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectCurrentSituationRepo : GenericRepo<ProjectCurrentSituation>
    {
        public ProjectCurrentSituationRepo(CurrentUser currentUser) : base(currentUser)
        {

        }

        public bool Validate(ProjectCurrentSituation item, out string message)
        {
            message = string.Empty;
            //check đã có yc báo giá chưa
          
                if (item.ProjectID <= 0)
                {
                    message = $"Vui lòng nhập dự án!";
                    return false;
                }
                if (item.ContentSituation == null || item.ContentSituation == "")
                {
                    message = $"Vui lòng nhập nội dung!";
                    return false;
                }
            return true;
        }
    }
}

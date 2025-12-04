using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class ProjectHistoryProblemRepo : GenericRepo<ProjectHistoryProblem>
    {
        private readonly ProjectHistoryProblemDetailRepo _historyDetailRepo;
        public ProjectHistoryProblemRepo(CurrentUser currentUser, ProjectHistoryProblemDetailRepo historyDetailRepo) : base(currentUser)
        {
            _historyDetailRepo = historyDetailRepo;
        }

        public bool Validate(List<ProjectHistoryProblemDTO> items, out string message)
        {
            int i = 0;
            message = string.Empty;
            foreach (var x in items)
            {
                if (x.projectHistoryProblem != null)
                {
                    i++;

                    if (string.IsNullOrEmpty(x.projectHistoryProblem.TypeProblem))
                    {
                        message = $"Vui lòng nhập Loại cho dòng thứ [{i}]";
                        return false;
                    }
                    if (string.IsNullOrEmpty(x.projectHistoryProblem.ContentError))
                    {
                        message = $"Vui lòng nhập Nội dung cho dòng thứ [{i}]";
                        return false;
                    }
                    if (string.IsNullOrEmpty(x.projectHistoryProblem.Reason))
                    {
                        message = $"Vui lòng nhập Nguyên nhân cho dòng thứ [{i}]";
                        return false;
                    }
                }
            }

            // --- Validate chi tiết nếu cần ---
            foreach (var y in items)
            {
                if (y.detail != null)
                {
                    foreach (var d in y.detail)
                    {
                        List<ProjectHistoryProblemDetail> p = _historyDetailRepo.GetAll(x => x.Description == d.Description);
                        ProjectHistoryProblem hp = GetByID(d.ProjectHistoryProblemID ?? 0);
                        if(d.ID <=0 && p.Count > 0)
                        {
                            message = $"Mô tả [{d.Description}] đã tồn tại ở nội dung lỗi [{hp.ContentError}], vui lòng nhập lại!";
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}

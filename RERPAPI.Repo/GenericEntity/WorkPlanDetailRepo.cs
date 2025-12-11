using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class WorkPlanDetailRepo : GenericRepo<WorkPlanDetail>
    {
        public WorkPlanDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public async Task<int> Create(WorkPlan plan)
        {
            try
            {
                var details = GetAll(x => x.WorkPlanID == plan.ID && x.IsDeleted != true);

                //Xóa hết ds chi tiết
                foreach (var item in details)
                {
                    item.IsDeleted = true;
                    await UpdateAsync(item);
                }
                details.Clear();
                //Insert lại 1 list mới
                if (plan.IsDeleted == false)
                {
                    for (int i = 0; i < plan.TotalDay; i++)
                    {
                        WorkPlanDetail detail = new WorkPlanDetail();
                        detail.UserID = plan.UserID;
                        detail.WorkContent = plan.WorkContent;
                        detail.DateDay = plan.StartDate.Value.AddDays(i);
                        detail.Location = plan.Location;
                        detail.ProjectID = plan.ProjectID;
                        detail.STT = i + 1;
                        detail.WorkPlanID = plan.ID;
                        details.Add(detail);
                    }
                }

                return await CreateRangeAsync(details);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

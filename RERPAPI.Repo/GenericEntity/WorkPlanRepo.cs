using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class WorkPlanRepo : GenericRepo<WorkPlan>
    {
        public WorkPlanRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public APIResponse Validate(WorkPlan plan)
        {

            try
            {
                var response = ApiResponseFactory.Success(null, "");

                if (!plan.StartDate.HasValue)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Ngày bắt đầu!");
                }

                if (!plan.EndDate.HasValue)
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Ngày kết thúc!");
                }

                plan.TotalDay = TextUtils.ToInt32((plan.EndDate.Value.Date - plan.StartDate.Value.Date).TotalDays) + 1;
                if (plan.TotalDay <= 0)
                {
                    response = ApiResponseFactory.Fail(null, "Ngày kết thúc phải lớn hơn Ngày bắt đầu!");
                }

                if (string.IsNullOrWhiteSpace(plan.Location))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Nơi làm việc!");
                }

                if (string.IsNullOrWhiteSpace(plan.WorkContent))
                {
                    response = ApiResponseFactory.Fail(null, "Vui lòng nhập Nội dung công việc!");
                }

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}

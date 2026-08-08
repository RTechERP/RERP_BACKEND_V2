using Dapper;
using Microsoft.Data.SqlClient;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepLinkRepo : GenericRepo<ProjectGateStepLink>
    {
        public ProjectGateStepLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        /// <summary>
        /// Kiểm tra xem trong danh sách công đoạn có checklist nào chưa được duyệt TBP (IsApprovedTBP != 1) không.
        /// Sử dụng SqlDapper để gọi stored procedure spCheckProjectGateStepPendingTbp.
        /// </summary>
        public async Task<bool> CheckPendingTbpAsync(List<int> linkIds)
        {
            if (linkIds == null || !linkIds.Any())
                return false;

            var param = new { LinkIDs = string.Join(",", linkIds) };
            var result = await SqlDapper<ProjectGateStepPendingCheckDto>.ProcedureToListTAsync(
                "spCheckProjectGateStepPendingTbp", 
                param
            );

            var pendingCount = result?.FirstOrDefault()?.PendingCount ?? 0;
            return pendingCount > 0;
        }
    }
}

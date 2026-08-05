using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPISaleApprovalRepo : GenericRepo<KPISaleApproval>
    {
        public KPISaleApprovalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        /// <summary>
        /// Lookup 1 bản ghi approval đang có (mới nhất) theo scope, refId và period.
        /// </summary>
        public async Task<KPISaleApproval?> GetCurrentAsync(string scope, int? employeeId, int? teamId, int periodId)
        {
            var query = table.AsNoTracking().AsQueryable();

            if (string.Equals(scope, "EMPLOYEE", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.ApprovalScope == "EMPLOYEE" && x.EmployeeID == employeeId);
            else if (string.Equals(scope, "TEAM", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.ApprovalScope == "TEAM" && x.TeamID == teamId);
            else
                return null;

            query = query.Where(x => x.PeriodID == periodId);

            return await query.OrderByDescending(x => x.ID).FirstOrDefaultAsync();
        }
    }
}

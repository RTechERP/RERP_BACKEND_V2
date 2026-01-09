using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Enum;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartListPurchaseRequestApproveLogRepo : GenericRepo<ProjectPartListPurchaseRequestApproveLog>
    {
        public ProjectPartListPurchaseRequestApproveLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public async Task CreateLogAsync( int projectPartlistPurchaseRequestId,PurchaseRequestApproveStatus status, int? employeeId,string? userName)
        {
            var log = new ProjectPartListPurchaseRequestApproveLog
            {
                ProjectPartlistPurchaseRequestID = projectPartlistPurchaseRequestId,
                Status = (int)status,
                EmployeeID = employeeId,
                DateStatus = DateTime.Now,
            };
            await CreateAsync(log);
        }
    }
}

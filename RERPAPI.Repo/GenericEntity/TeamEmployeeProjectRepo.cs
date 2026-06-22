using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Common;

namespace RERPAPI.Repo.GenericEntity
{
    public class TeamEmployeeProjectRepo : GenericRepo<Employee>
    {
        public TeamEmployeeProjectRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public async Task<List<TeamProjectParticipationDto>> GetProjectsByEmployees(
            List<int> employeeIds,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            if (employeeIds == null || !employeeIds.Any())
                return new List<TeamProjectParticipationDto>();

            var param = new
            {
                EmployeeIDs = string.Join(",", employeeIds),
                DateFrom = dateFrom,
                DateTo = dateTo
            };

            return await SqlDapper<TeamProjectParticipationDto>.ProcedureToListTAsync("spGetProjectsByEmployees", param);
        }
    }
}

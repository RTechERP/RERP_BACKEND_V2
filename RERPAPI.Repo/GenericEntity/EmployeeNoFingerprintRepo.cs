using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeNoFingerprintRepo : GenericRepo<EmployeeNoFingerprint>
    {
        public EmployeeNoFingerprintRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
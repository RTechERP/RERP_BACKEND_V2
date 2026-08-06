using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class EmployeeSignatureFileRepo : GenericRepo<EmployeeSignatureFile>
    {
        public EmployeeSignatureFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
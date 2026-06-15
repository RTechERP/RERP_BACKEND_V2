using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormOtherCertificateRepo : GenericRepo<HRHiringCandidateInformationFormOtherCertificate>
    {
        public HRHiringCandidateInformationFormOtherCertificateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
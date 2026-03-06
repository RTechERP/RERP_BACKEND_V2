using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormOtherCertificateRepo:GenericRepo<HRHiringCandidateInformationFormOtherCertificate>
    {
        public HRHiringCandidateInformationFormOtherCertificateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

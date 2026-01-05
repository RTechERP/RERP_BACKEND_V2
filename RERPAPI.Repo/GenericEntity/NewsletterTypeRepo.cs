using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class NewsletterTypeRepo : GenericRepo<NewsletterType>
    {
        private PhasedAllocationPersonRepo phaseRepo;
        private PhasedAllocationPersonDetailRepo phaseDetailRepo;

        public NewsletterTypeRepo(CurrentUser currentUser, PhasedAllocationPersonRepo phaseRepo, PhasedAllocationPersonDetailRepo phaseDetailRepo) : base(currentUser)
        {
            this.phaseRepo = phaseRepo;
            this.phaseDetailRepo = phaseDetailRepo;
        }

    }
}

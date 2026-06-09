using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingDepartmentLinkRepo: GenericRepo<MakerTrainingDepartmentLink>
    {
        public MakerTrainingDepartmentLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

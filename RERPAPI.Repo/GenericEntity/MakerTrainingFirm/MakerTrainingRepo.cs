using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.MakerTrainingFirm
{
    public class MakerTrainingRepo: GenericRepo<MakerTraining>
    {
        public MakerTrainingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}

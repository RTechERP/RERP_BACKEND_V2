using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class MakerTrainingDTO
    {
        public MakerTraining? MakerTraining { get; set; }
        public List<MakerTrainingDocument>? MakerTrainingDocument { get; set; }
        public List<MakerTrainingEmployeeLink>? MakerTrainingEmployeeLink { get; set; }
        public List<int>? DeletedFileIds { get; set; }

        public List<int>? DeletedEmployees { get; set; }
    }
}

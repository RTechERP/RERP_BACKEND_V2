using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
   public class TrainingRegistrationDTO:TrainingRegistration
   {
        public List<TrainingRegistrationFile> LstFile { get; set; }
        public List<TrainingRegistrationDetail> LstDetail { get; set; }
    }
}

using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class TrainingRegistrationDTO : TrainingRegistration
    {
        public List<TrainingRegistrationFile> LstFile { get; set; }
        public List<TrainingRegistrationDetail> LstDetail { get; set; }
    }
}
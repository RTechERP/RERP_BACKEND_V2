using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM;

public class HRRecruitmentApplicationFullDTO
{
    public HRRecruitmentApplicationForm HRRecruitmentApplicationForm { get; set; } = null!;
    public List<HRHiringCandidateInformationFormWorkingExperience>? WorkingExperiences { get; set; }
    public List<HRHiringCandidateInformationFormOtherCertificate>? OtherCertificates { get; set; }
    public List<HRHiringCandidateInformationFormEducation>? Educations { get; set; }
    public List<HRHiringCandidateInformationEmergencyContact>? EmergencyContacts { get; set; }
    public List<HRHiringCandidateInformationFormForeignLanguageSkill>? ForeignLanguageSkills { get; set; }
    public HRHiringCandidateInformationFormRecruitmentInfo? RecruitmentInfo { get; set; }
}

using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.HRM
{
    public class HRHiringRequestDTO
    {
        // Main entity
        public HRHiringRequest HiringRequests { get; set; }

        // Link data - tất cả dùng số
        public List<int>? EducationLevels { get; set; }
        public List<int>? Experiences { get; set; }
        public List<int>? Appearances { get; set; }
        public List<int>? Genders { get; set; }
        public List<HealthDTO>? HealthRequirements { get; set; }
        public List<LanguageDTO>? Languages { get; set; } // Ngoại ngữ cần string
        public List<ComputerSkillDTO>? ComputerSkills { get; set; }
        public List<CommunicationDTO>? Communications { get; set; }
        public List<ApprovalDTO>? Approvals { get; set; }
    }

    // Language DTO - vì cần lưu tên và level
    // Sửa lại LanguageDTO
    public class LanguageDTO
    {
        public int LanguageType { get; set; } // 1: Tiếng Anh, 2: Khác
        public string? LanguageTypeName { get; set; } // "Tiếng Anh" hoặc tên ngôn ngữ khác
        public int LanguageLevel { get; set; } // 1: Level A, 2: Level B, 3: Level C, 4: Không cần thiết
    }

    // Computer Skill DTO - sửa lại

    public class ComputerSkillDTO
    {
        public int ComputerType { get; set; } // 1-6
        public string? ComputerName { get; set; }

    }

    // Communication DTO - giữ nguyên
    public class CommunicationDTO
    {
        public int CommunicationType { get; set; }
        public string? CommunicationDecription { get; set; }
    }

    public class HealthDTO
    {
        public int HealthType { get; set; }
        public string? HealthDescription { get; set; }
    }


    // Approval DTO - giữ nguyên
    public class ApprovalDTO
    {
        public int ApproveID { get; set; }
        public int Step { get; set; }
        public string? StepName { get; set; }
        public string? Note { get; set; }
    }
}

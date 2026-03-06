using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormRepo : GenericRepo<HRRecruitmentApplicationForm>
    {
        public HRHiringCandidateInformationFormRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public APIResponse Validate(HRRecruitmentApplicationFullDTO data)
        {
            try
            {
                var mainForm = data.HRRecruitmentApplicationForm;
                if (mainForm == null) return ApiResponseFactory.Fail(null, "Không có dữ liệu tờ khai!");
                // Check required fields in main form (skipping CCCD, IssuedOn, IssuedBy, Hobbies, Height, Weight, OtherActivities)
                if (mainForm.ChucVuHDID <= 0) return ApiResponseFactory.Fail(null, "Vui lòng chọn Vị trí dự tuyển!");
                if (string.IsNullOrWhiteSpace(mainForm.FullName)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Họ và tên!");
                if (mainForm.Gender <= 0) return ApiResponseFactory.Fail(null, "Vui lòng chọn Giới tính!");
                if (!mainForm.DateOfBirth.HasValue) return ApiResponseFactory.Fail(null, "Vui lòng chọn Ngày sinh!");
                if (string.IsNullOrWhiteSpace(mainForm.PlaceOfBirth)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Nơi sinh!");
                if (string.IsNullOrWhiteSpace(mainForm.Ethnic)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Dân tộc!");
                if (string.IsNullOrWhiteSpace(mainForm.Religion)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Tôn giáo!");
                if (string.IsNullOrWhiteSpace(mainForm.PermanentResidence)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Thường trú!");
                if (string.IsNullOrWhiteSpace(mainForm.CurrentAddress)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Nơi ở hiện nay!");
                if (string.IsNullOrWhiteSpace(mainForm.Tel)) return ApiResponseFactory.Fail(null, "Vui lòng nhập ĐTCĐ!");
                if (string.IsNullOrWhiteSpace(mainForm.Mobile)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Di động!");
                if (string.IsNullOrWhiteSpace(mainForm.Email)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Email!");
                if (mainForm.MaritalStatus <= 0) return ApiResponseFactory.Fail(null, "Vui lòng chọn Tình trạng hôn nhân!");
                if (string.IsNullOrWhiteSpace(mainForm.Experiences)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Đặc điểm cá nhân và kinh nghiệm!");
                if (string.IsNullOrWhiteSpace(mainForm.ReasonApplication)) return ApiResponseFactory.Fail(null, "Vui lòng nhập Lý do nộp đơn!");
                if (mainForm.AcceptedSalary <= 0) return ApiResponseFactory.Fail(null, "Vui lòng nhập Mức lương mong muốn!");
                if (!mainForm.DateOfStart.HasValue) return ApiResponseFactory.Fail(null, "Vui lòng chọn Ngày có thể bắt đầu làm việc!");

                // Survey Questions Validation
                if (mainForm.HasRelativeOrFriendInCompany == true && string.IsNullOrWhiteSpace(mainForm.RelativeInfo))
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập thông tin người thân/bạn bè tại công ty!");
                if (mainForm.HasSocialInsurance == true && string.IsNullOrWhiteSpace(mainForm.BHXH))
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập số sổ Bảo hiểm xã hội!");
                if (mainForm.HasTaxCode == true && string.IsNullOrWhiteSpace(mainForm.TaxCode))
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập mã số thuế cá nhân!");
                // Check Emergency Contacts (at least 2)
                if (data.EmergencyContacts == null || data.EmergencyContacts.Count < 2)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng cung cấp ít nhất 2 Người liên hệ khẩn khi cần!");
                }
                foreach (var contact in data.EmergencyContacts)
                {
                    if (string.IsNullOrWhiteSpace(contact.FullName) || string.IsNullOrWhiteSpace(contact.Relation) ||
                        string.IsNullOrWhiteSpace(contact.Tel) || string.IsNullOrWhiteSpace(contact.Address))
                    {
                        return ApiResponseFactory.Fail(null, "Vui lòng nhập đầy đủ thông tin cho Người liên hệ khẩn cấp!");
                    }
                }
                // Check Educations (at least 1)
                if (data.Educations == null || data.Educations.Count < 1)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng cung cấp ít nhất 1 thông tin Trình độ học vấn!");
                }
                foreach (var edu in data.Educations)
                {
                    if (string.IsNullOrWhiteSpace(edu.NameOfSchool) || string.IsNullOrWhiteSpace(edu.Major) ||
                        string.IsNullOrWhiteSpace(edu.GraduatedTime) || edu.QualificationLevel <= 0)
                    {
                        return ApiResponseFactory.Fail(null, "Vui lòng nhập đầy đủ thông tin cho Trình độ học vấn!");
                    }
                }
                // Work Experiences, Foreign Languages, Other Certificates: skipped by user request
                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RERPAPI.Model.Common;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRRecruitmentCandidateRepo : GenericRepo<HRRecruitmentCandidate>
    {
        private readonly SmtpSettings _smtp;

        HRRecruitmentCandidateLogRepo _hrRecruitmentCandidateLogRepo;

        public HRRecruitmentCandidateRepo(
            CurrentUser currentUser,
            HRRecruitmentCandidateLogRepo hRRecruitmentCandidateLogRepo, IOptions<SmtpSettings> smtp) : base(currentUser)
        {
            _hrRecruitmentCandidateLogRepo = hRRecruitmentCandidateLogRepo;
            _smtp = smtp.Value;
        }
        public string GenerateUserName()
        {
            int stt = GetAll().Count+1;
            return $"UV000{stt}";
        }

        public bool Validate(HRRecruitmentCandidateDTO data, out string message)
        {
            message = string.Empty;
            var checkExist = this.GetAll(
                    x => x.ID != data.ID &&
                    x.UserName.ToLower().Trim() == data.UserName.ToLower().Trim() &&
                    x.IsDeleted != true).FirstOrDefault();

            string fileCVName = data.FileCVName ?? "";
            var checkExistFile = this.GetAll(
                    x => x.ID != data.ID &&
                    x.FileCVName.ToLower().Trim() == fileCVName.ToLower().Trim() &&
                    x.IsDeleted != true).FirstOrDefault();

            if (checkExist != null)
            {
                message = $"Mã ứng viên {data.UserName} đã tồn tại! Vui lòng nhập mã ứng viên khác.";
                return false;
            }

            if (checkExistFile != null && !String.IsNullOrWhiteSpace(fileCVName))
            {
                message = $"FIle Cv {data.FileCVName} đã tồn tại cho ứng viên {checkExistFile.FullName}! Vui lòng kiểm tra lại.";
                return false;
            }

            if (data.Status >= 0 && data.ID > 0)
            {
                var checkExistLog = _hrRecruitmentCandidateLogRepo.GetAll(
                    x => x.HRRecruitmentCandidateID == data.ID &&
                    x.IsDeleted != true && x.IsApproved == true).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                if (checkExistLog != null && data.Status < checkExistLog.ApprovedStep)
                {
                    message = $"Vui lòng hủy trạng thái hiện tại trước khi update trạng thái trước đó.";
                    return false;
                }
            }

            return true;
        }

        public string GetFooterMail(int status)
        {

            if (status == 0)
                return string.Empty;

            return $@"
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-family: Arial, Helvetica, sans-serif; font-size: 13px; color:#000;"">
    <tr>
        <td style=""padding:15px 0;"">
            <table cellpadding=""0"" cellspacing=""0"">
                <tr>
                    <td style=""padding-right:15px; border-right:2px solid #f36f21;"">
                        <img
                          src=""https://erp.rtc.edu.vn/rerpweb/assets/images/logo-RTC-2023-1200-banchuan.png""
                          width=""150""
                          alt=""RTC Technology""
                          style=""
                            display:block;
                            max-width:150px;
                            height:auto;
                          ""
                        />
                    </td>

                    <td style=""padding-left:15px; line-height:1.6;"">
                        <div style=""color:#0070c0; font-weight:bold;"">
                            Phòng Hành chính – Nhân sự
                        </div>
                        <div>
                            Email:
                            <a href=""mailto:hr@rtc.edu.vn"" style=""color:#0070c0; text-decoration:none;"">
                                hr@rtc.edu.vn
                            </a>
                        </div>
                        <div>
                            Phone: (+84) 965 513 189
                        </div>

                        <hr style=""border:none; border-top:1px dashed #999; margin:8px 0;"">

                        <div style=""font-weight:bold;"">
                            CÔNG TY CỔ PHẦN RTC TECHNOLOGY VIỆT NAM
                        </div>

                        <div style=""color:#e74c3c;"">
                            <b>HANOI HEAD OFFICE:</b>
                            Floor 1, Area P - Hateco Apollo Building,
                            Phuong Canh Ward, Nam Tu Liem District, Ha Noi, Vietnam.
                        </div>

                        <div style=""color:#27ae60;"">
                            <b>BACNINH OFFICE:</b>
                            LK 1-8/OTM1 O Cach Village, Dong Tien commune,
                            Yen Phong District, Bac Ninh Province, Vietnam.
                        </div>

                        <div style=""color:#2980b9;"">
                            <b>HOCHIMINH OFFICE:</b>
                            No.43, Road No.01, Phu Huu Urban Area,
                            Thu Duc City, Ho Chi Minh, Vietnam.
                        </div>

                        <div>
                            Website:
                            <a href=""https://rtc.edu.vn"" style=""color:#0070c0;"">www.rtc.edu.vn</a> -
                            <a href=""https://rtctechnology.com.vn"" style=""color:#0070c0;"">www.rtctechnology.com.vn</a> -
                            <a href=""https://agv-smart.com"" style=""color:#0070c0;"">www.agv-smart.com</a>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>";
        }
    }
}

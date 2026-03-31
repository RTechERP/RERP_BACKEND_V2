using RERPAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class SendEmailReceiveProjectTaskParam
    {
        public string emailRecive { get; set; }
        public string nameRecive { get; set; }
        public string nameAsigner { get; set; }
        public string projectTaskName { get; set; }
        public string projectTaskCode { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string role { get; set; }
        public string discription { get; set; }
        public string emailCC { get; set; }
    }

    public class SendEmailReceiveProjectTaskClass
    {
        public  readonly EmailHelper _emailHelper;
        public SendEmailReceiveProjectTaskClass(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }
        public async Task SendEmailReceiveProjectTask(SendEmailReceiveProjectTaskParam sendEmail)
        {
            try
            {

                string htmlBody = @"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo thêm vào công việc</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333333;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }
        .email-container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .header {
            background-color: #4a90e2;
            color: white;
            padding: 30px 20px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }
        .content {
            padding: 30px;
        }
        .greeting {
            font-size: 18px;
            margin-bottom: 20px;
        }
        .project-details {
            background-color: #f8f9fa;
            border-left: 4px solid #4a90e2;
            padding: 20px;
            margin: 25px 0;
            border-radius: 4px;
        }
        .project-name {
            font-size: 22px;
            font-weight: 600;
            color: #4a90e2;
            margin-bottom: 10px;
        }
        .project-info {
            margin: 5px 0;
            color: #555555;
        }
        .button {
            display: inline-block;
            background-color: #4a90e2;
            color: white !important;
            text-decoration: none;
            padding: 12px 30px;
            border-radius: 5px;
            font-weight: 500;
            margin: 20px 0;
        }
        .footer {
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            color: #888888;
            border-top: 1px solid #e0e0e0;
        }
        .divider {
            height: 1px;
            background-color: #e0e0e0;
            margin: 25px 0;
        }
        .highlight {
            color: #4a90e2;
            font-weight: 600;
        }
        @media only screen and (max-width: 600px) {
            .content {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <!-- Header -->
        <div class=""header"">
            <h1>🎉 THÔNG BÁO THAM GIA CÔNG VIỆC</h1>
        </div>

        <!-- Content -->
        <div class=""content"">
            <div class=""greeting"">
                Xin chào bạn!,
            </div>
            
            <p>Chúng tôi xin thông báo rằng bạn đã được thêm vào công việc:</p>
            
            <!-- Project Details -->
            <div class=""project-details"">
                <div class=""project-name"">" + sendEmail.projectTaskName + @"</div>
                <div class=""project-info""><strong>Mã công việc: </strong>" + sendEmail.projectTaskCode + @"</div>
                <div class=""project-info""><strong>Người giao việc: </strong>" + sendEmail.nameAsigner + @"</div>
                <div class=""project-info""><strong>Ngày bắt đầu:</strong> " + sendEmail.startDate + @"</div>
                <div class=""project-info""><strong>Hạn hoàn thành:</strong>" + sendEmail.endDate + @"</div>
                <div class=""project-info""><strong>Mô tả công việc:</strong>
                <p>" + sendEmail.discription + @"</p></div>
            </div> 

            <!-- Action Button -->
            <div style=""text-align: center;"">
                <a href=""https://erp.rtc.edu.vn//rerpweb//project-task"" class=""button"">XEM CHI TIẾT CÔNG VIỆC</a>
            </div>

            <!-- Additional Info -->
            <p style=""font-style: italic; color: #666666;"">
                💡 Vui lòng đăng nhập vào hệ thống để xem thông tin chi tiết và cập nhật tiến độ công việc. 
                Nếu có bất kỳ thắc mắc nào, hãy liên hệ với quản lý dự án hoặc bộ phận hỗ trợ.
            </p>

            <p>
                Chúc bạn có những trải nghiệm làm việc thú vị và hiệu quả!<br>
                Trân trọng,<br>
                <strong>RTC ERP</strong>
            </p>
        </div>

    </div>
</body>
</html>";


                // await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO THAM GIA CÔNG VIỆC", htmlBody, cc: sendEmail.emailCC);
               await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO THAM GIA CÔNG VIỆC", htmlBody, cc: sendEmail.emailCC);
                

            }
            catch (Exception ex)
            {

            }
        }
    }
}

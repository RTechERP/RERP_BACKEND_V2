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
        public string? nameRecive { get; set; }
        public string? nameAsigner { get; set; }
        public string? projectTaskName { get; set; }
        public string? projectTaskCode { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? completedDate { get; set; }
        public int? reviewStatus { get; set; } // 1: ĐUYỆT HOÀN THÀNH, 2: HỦY DUYỆT
        public string? reviewDate { get; set; }
        public string? reviewDiscription { get; set; }
        public string? role { get; set; }
        public string? discription { get; set; }
        public string? emailCC { get; set; }
    }

    public class SendEmailReceiveProjectTaskClass
    {
        public readonly EmailHelper _emailHelper;
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
                <a href=""https://erp.rtc.edu.vn/rerpweb/project-task"" class=""button"">XEM CHI TIẾT CÔNG VIỆC</a>
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

        public async Task SendEmailCompleteProjectTask(SendEmailReceiveProjectTaskParam sendEmail)
        {
            try
            {

                string htmlBody = @"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo hoàn thành công việc</title>
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
            max-width: 560px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .header {
            background-color: #2e7d32;
            padding: 28px 24px;
            text-align: center;
        }
        .header-icon {
            width: 52px;
            height: 52px;
            border-radius: 50%;
            background-color: rgba(255,255,255,0.2);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            margin-bottom: 12px;
        }
        .header h1 {
            margin: 0;
            font-size: 20px;
            font-weight: 500;
            color: #ffffff;
            letter-spacing: 0.3px;
        }
        .header p {
            margin: 6px 0 0;
            font-size: 13px;
            color: rgba(255,255,255,0.75);
        }
        .content {
            padding: 28px;
        }
        .greeting {
            font-size: 15px;
            margin-bottom: 16px;
            color: #333333;
        }
        .intro {
            font-size: 14px;
            color: #666666;
            margin-bottom: 20px;
            line-height: 1.7;
        }
        .task-details {
            background-color: #f8f9fa;
            border-left: 3px solid #2e7d32;
            border-radius: 6px;
            padding: 16px 20px;
            margin-bottom: 20px;
        }
        .task-name {
            font-size: 16px;
            font-weight: 500;
            color: #1a1a1a;
            margin-bottom: 12px;
        }
        .task-row {
            display: flex;
            gap: 8px;
            font-size: 13px;
            margin-bottom: 8px;
        }
        .task-label {
            color: #888888;
            min-width: 130px;
        }
        .task-value {
            color: #333333;
            font-weight: 500;
        }
        .status-badge {
            display: inline-block;
            background-color: #e8f5e9;
            color: #2e7d32;
            font-size: 12px;
            font-weight: 500;
            padding: 2px 10px;
            border-radius: 20px;
        }
        .btn-wrapper {
            text-align: center;
            margin-bottom: 20px;
        }
        .button {
            display: inline-block;
            background-color: #2e7d32;
            color: #ffffff !important;
            text-decoration: none;
            padding: 10px 28px;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 500;
        }
        .note {
            font-size: 13px;
            color: #888888;
            line-height: 1.6;
            font-style: italic;
        }
        .footer {
            border-top: 1px solid #eeeeee;
            padding: 14px 28px;
            text-align: center;
            font-size: 12px;
            color: #aaaaaa;
        }
        .footer strong {
            color: #555555;
        }
    </style>
</head>
<body>
    <div class=""email-container"">

        <div class=""header"">
            <div class=""header-icon"">✓</div>
            <h1>Công việc đã hoàn thành</h1>
            <p>Thông báo tự động từ RTC ERP</p>
        </div>

        <div class=""content"">
            <div class=""greeting"">Xin chào,</div>

            <p class=""intro"">
                Chúng tôi xin thông báo rằng công việc <strong>" + sendEmail.projectTaskName + @"</strong>
                đã được hoàn thành. Chi tiết như sau:
            </p>

            <div class=""task-details"">
                <div class=""task-name"">" + sendEmail.projectTaskName + @"</div>
                <div class=""task-row"">
                    <span class=""task-label"">Mã công việc</span>
                    <span class=""task-value"">" + sendEmail.projectTaskCode + @"</span>
                </div>
                <div class=""task-row"">
                    <span class=""task-label"">Người giao việc</span>
                    <span class=""task-value"">" + sendEmail.nameAsigner + @"</span>
                </div>
                <div class=""task-row"">
                    <span class=""task-label"">Ngày hoàn thành</span>
                    <span class=""task-value"">" + sendEmail.completedDate + @"</span>
                </div>
                <div class=""task-row"">
                    <span class=""task-label"">Trạng thái</span>
                    <span class=""task-value""><span class=""status-badge"">Hoàn thành</span></span>
                </div>
            </div>

            <div class=""btn-wrapper"">
                <a href=""https://erp.rtc.edu.vn/rerpweb/project-task"" class=""button"">Xem chi tiết công việc</a>
            </div>

            <p class=""note"">
                💡 Vui lòng đăng nhập hệ thống để xem báo cáo đầy đủ và xác nhận kết quả công việc.
                Nếu có thắc mắc, hãy liên hệ quản lý dự án hoặc bộ phận hỗ trợ.
            </p>
        </div>

        <div class=""footer"">
            Trân trọng — <strong>RTC ERP</strong>
        </div>

    </div>
</body>
</html>";


                // await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO THAM GIA CÔNG VIỆC", htmlBody, cc: sendEmail.emailCC);
                await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO CÔNG VIỆC HOÀN THÀNH", htmlBody, cc: sendEmail.emailCC);


            }
            catch (Exception ex)
            {

            }
        }

        public async Task SendEmailCompleteReviewProjectTask(SendEmailReceiveProjectTaskParam sendEmail)
        {
            try
            {

                string htmlBody = @"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông báo duyệt hoàn thành công việc</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333333; margin: 0; padding: 0; background-color: #f5f5f5; }
        .email-container { max-width: 560px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.08); }
        .header { background-color: #1565c0; padding: 28px 24px; text-align: center; }
        .header-icon { width: 52px; height: 52px; border-radius: 50%; background-color: rgba(255,255,255,0.2); display: inline-flex; align-items: center; justify-content: center; font-size: 22px; margin-bottom: 12px; }
        .header h1 { margin: 0; font-size: 20px; font-weight: 500; color: #ffffff; letter-spacing: 0.3px; }
        .header p { margin: 6px 0 0; font-size: 13px; color: rgba(255,255,255,0.75); }
        .content { padding: 28px; }
        .greeting { font-size: 15px; margin-bottom: 16px; color: #333333; }
        .intro { font-size: 14px; color: #666666; margin-bottom: 20px; line-height: 1.7; }
        .task-details { background-color: #f8f9fa; border-left: 3px solid #1565c0; border-radius: 6px; padding: 16px 20px; margin-bottom: 16px; }
        .task-name { font-size: 16px; font-weight: 500; color: #1a1a1a; margin-bottom: 12px; }
        .task-row { display: flex; gap: 8px; font-size: 13px; margin-bottom: 8px; align-items: center; }
        .task-label { color: #888888; min-width: 130px; }
        .task-value { color: #333333; font-weight: 500; }
        .status-badge { display: inline-block; background-color: #e3f2fd; color: #1565c0; font-size: 12px; font-weight: 500; padding: 2px 10px; border-radius: 20px; }
        .comment-box { background-color: #f0f7ff; border: 1px solid #bbdefb; border-radius: 6px; padding: 14px 18px; margin-bottom: 20px; }
        .comment-label { font-size: 12px; color: #1565c0; font-weight: 500; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 6px; }
        .comment-text { font-size: 13px; color: #555555; line-height: 1.6; margin: 0; }
        .btn-wrapper { text-align: center; margin-bottom: 20px; }
        .button { display: inline-block; background-color: #1565c0; color: #ffffff !important; text-decoration: none; padding: 10px 28px; border-radius: 6px; font-size: 14px; font-weight: 500; }
        .note { font-size: 13px; color: #888888; line-height: 1.6; font-style: italic; }
        .footer { border-top: 1px solid #eeeeee; padding: 14px 28px; text-align: center; font-size: 12px; color: #aaaaaa; }
        .footer strong { color: #555555; }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""header-icon"">★</div>
            <h1>Công việc đã được duyệt</h1>
            <p>Thông báo tự động từ RTC ERP</p>
        </div>
        <div class=""content"">
            <div class=""greeting"">Xin chào,</div>
            <p class=""intro"">Công việc <strong>" + sendEmail.projectTaskName + @"</strong> đã được phê duyệt hoàn thành. Chi tiết như sau:</p>
            <div class=""task-details"">
                <div class=""task-name"">" + sendEmail.projectTaskName + @"</div>
                <div class=""task-row"">
                    <span class=""task-label"">Mã công việc</span>
                    <span class=""task-value"">" + sendEmail.projectTaskCode + @"</span>
                </div>
                <div class=""task-row"">
                    <span class=""task-label"">Người duyệt</span>
                    <span class=""task-value"">" + sendEmail.nameAsigner + @"</span>
                </div>
                <div class=""task-row"">
                    <span class=""task-label"">Ngày duyệt</span>
                    <span class=""task-value"">" + sendEmail.reviewDate + @"</span>
                </div>
                <div class=""task-row"">
                    <span class=""task-label"">Trạng thái</span>
                    <span class=""task-value""><span class=""status-badge"">Đã duyệt hoàn thành</span></span>
                </div>
            </div>
            <div class=""comment-box"">
                <div class=""comment-label"">Nhận xét</div>
                <p class=""comment-text"">" + sendEmail.reviewDiscription + @"</p>
            </div>
            <div class=""btn-wrapper"">
                <a href=""https://erp.rtc.edu.vn/rerpweb/project-task"" class=""button"">Xem chi tiết công việc</a>
            </div>
            <p class=""note"">💡 Vui lòng đăng nhập hệ thống để xem kết quả phê duyệt và lưu trữ hồ sơ công việc. Nếu có thắc mắc, hãy liên hệ quản lý dự án hoặc bộ phận hỗ trợ.</p>
        </div>
        <div class=""footer"">Trân trọng — <strong>RTC ERP</strong></div>
    </div>
</body>
</html>";


                // await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO THAM GIA CÔNG VIỆC", htmlBody, cc: sendEmail.emailCC);
                await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO CÔNG VIỆC ĐÃ ĐƯỢC DUYỆT ", htmlBody, cc: sendEmail.emailCC);


            }
            catch (Exception ex)
            {

            }
        }
        public async Task SendEmailRejectReviewProjectTask(SendEmailReceiveProjectTaskParam sendEmail)
        {
            try
            {

                string htmlBody = @"<!DOCTYPE html>
                    <html lang=""vi"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Thông báo hủy duyệt công việc</title>
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
                                max-width: 560px;
                                margin: 20px auto;
                                background-color: #ffffff;
                                border-radius: 10px;
                                overflow: hidden;
                                box-shadow: 0 2px 10px rgba(0,0,0,0.08);
                            }
                            .header {
                                background-color: #c62828;
                                padding: 28px 24px;
                                text-align: center;
                            }
                            .header-icon {
                                width: 52px;
                                height: 52px;
                                border-radius: 50%;
                                background-color: rgba(255,255,255,0.2);
                                display: inline-flex;
                                align-items: center;
                                justify-content: center;
                                font-size: 22px;
                                margin-bottom: 12px;
                            }
                            .header h1 {
                                margin: 0;
                                font-size: 20px;
                                font-weight: 500;
                                color: #ffffff;
                                letter-spacing: 0.3px;
                            }
                            .header p {
                                margin: 6px 0 0;
                                font-size: 13px;
                                color: rgba(255,255,255,0.75);
                            }
                            .content {
                                padding: 28px;
                            }
                            .greeting {
                                font-size: 15px;
                                margin-bottom: 16px;
                                color: #333333;
                            }
                            .intro {
                                font-size: 14px;
                                color: #666666;
                                margin-bottom: 20px;
                                line-height: 1.7;
                            }
                            .task-details {
                                background-color: #f8f9fa;
                                border-left: 3px solid #c62828;
                                border-radius: 6px;
                                padding: 16px 20px;
                                margin-bottom: 16px;
                            }
                            .task-name {
                                font-size: 16px;
                                font-weight: 500;
                                color: #1a1a1a;
                                margin-bottom: 12px;
                            }
                            .task-row {
                                display: flex;
                                gap: 8px;
                                font-size: 13px;
                                margin-bottom: 8px;
                                align-items: center;
                            }
                            .task-label {
                                color: #888888;
                                min-width: 130px;
                            }
                            .task-value {
                                color: #333333;
                                font-weight: 500;
                            }
                            .status-badge {
                                display: inline-block;
                                background-color: #ffebee;
                                color: #c62828;
                                font-size: 12px;
                                font-weight: 500;
                                padding: 2px 10px;
                                border-radius: 20px;
                            }
                            .reason-box {
                                background-color: #fff8f8;
                                border: 1px solid #ffcdd2;
                                border-radius: 6px;
                                padding: 14px 18px;
                                margin-bottom: 20px;
                            }
                            .reason-label {
                                font-size: 12px;
                                color: #c62828;
                                font-weight: 500;
                                text-transform: uppercase;
                                letter-spacing: 0.5px;
                                margin-bottom: 6px;
                            }
                            .reason-text {
                                font-size: 13px;
                                color: #555555;
                                line-height: 1.6;
                                margin: 0;
                            }
                            .btn-wrapper {
                                text-align: center;
                                margin-bottom: 20px;
                            }
                            .button {
                                display: inline-block;
                                background-color: #c62828;
                                color: #ffffff !important;
                                text-decoration: none;
                                padding: 10px 28px;
                                border-radius: 6px;
                                font-size: 14px;
                                font-weight: 500;
                            }
                            .note {
                                font-size: 13px;
                                color: #888888;
                                line-height: 1.6;
                                font-style: italic;
                            }
                            .footer {
                                border-top: 1px solid #eeeeee;
                                padding: 14px 28px;
                                text-align: center;
                                font-size: 12px;
                                color: #aaaaaa;
                            }
                            .footer strong {
                                color: #555555;
                            }
                        </style>
                    </head>
                    <body>
                        <div class=""email-container"">

                            <div class=""header"">
                                <div class=""header-icon"">✕</div>
                                <h1>Công việc bị hủy duyệt</h1>
                                <p>Thông báo tự động từ RTC ERP</p>
                            </div>

                            <div class=""content"">
                                <div class=""greeting"">Xin chào,</div>

                                <p class=""intro"">
                                    Công việc <strong>" + sendEmail.projectTaskName + @"</strong> đã bị hủy duyệt.
                                    Chi tiết như sau:
                                </p>

                                <div class=""task-details"">
                                    <div class=""task-name"">" + sendEmail.projectTaskName + @"</div>
                                    <div class=""task-row"">
                                        <span class=""task-label"">Mã công việc</span>
                                        <span class=""task-value"">" + sendEmail.projectTaskCode + @"</span>
                                    </div>
                                    <div class=""task-row"">
                                        <span class=""task-label"">Người hủy duyệt</span>
                                        <span class=""task-value"">" + sendEmail.nameAsigner + @"</span>
                                    </div>
                                    <div class=""task-row"">
                                        <span class=""task-label"">Ngày hủy duyệt</span>
                                        <span class=""task-value"">" + sendEmail.reviewDate + @"</span>
                                    </div>
                                    <div class=""task-row"">
                                        <span class=""task-label"">Trạng thái</span>
                                        <span class=""task-value""><span class=""status-badge"">Đã hủy duyệt</span></span>
                                    </div>
                                </div>

                                <div class=""reason-box"">
                                    <div class=""reason-label"">Lý do hủy duyệt</div>
                                    <p class=""reason-text"">" + sendEmail.reviewDate + @"</p>
                                </div>

                                <div class=""btn-wrapper"">
                                    <a href=""https://erp.rtc.edu.vn/rerpweb/project-task"" class=""button"">Xem chi tiết công việc</a>
                                </div>

                                <p class=""note"">
                                    💡 Vui lòng đăng nhập hệ thống để xem chi tiết và tiến hành cập nhật lại công việc theo yêu cầu.
                                    Nếu có thắc mắc, hãy liên hệ quản lý dự án hoặc bộ phận hỗ trợ.
                                </p>
                            </div>

                            <div class=""footer"">
                                Trân trọng — <strong>RTC ERP</strong>
                            </div>

                        </div>
                    </body>
                    </html>";


                // await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO THAM GIA CÔNG VIỆC", htmlBody, cc: sendEmail.emailCC);
                await _emailHelper.SendRangeAsync(sendEmail.emailRecive, "THÔNG BÁO CÔNG VIỆC ĐÃ HỦY DUYỆT ", htmlBody, cc: sendEmail.emailCC);


            }
            catch (Exception ex)
            {

            }
        }
    }
}

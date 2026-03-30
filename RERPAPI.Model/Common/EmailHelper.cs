using Microsoft.Extensions.Options;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.Data.SqlClient;

namespace RERPAPI.Model.Common
{
    public class EmailHelper
    {
        private readonly SmtpSettings _smtp;
        private readonly SmtpSettingsHr _smtpHr;

        public EmailHelper(IOptions<SmtpSettings> smtp, IOptions<SmtpSettingsHr> smtpHr)
        {
            _smtp = smtp.Value;
            _smtpHr = smtpHr.Value;
        }
        //public static async Task SendAsync(string[] toEmails, string subject, string htmlBody)
        //{
        //	try
        //	{
        //		if (string.IsNullOrWhiteSpace(Config.SmtpHost) || string.IsNullOrWhiteSpace(Config.SmtpFrom))
        //		{
        //			// Chưa cấu hình SMTP => bỏ qua gửi mail
        //			return;
        //		}

        //		using (var message = new MailMessage())
        //		{
        //			message.From = new MailAddress(Config.SmtpFrom, Config.SmtpFromDisplay ?? Config.SmtpFrom);
        //			foreach (var to in toEmails ?? Array.Empty<string>())
        //			{
        //				if (!string.IsNullOrWhiteSpace(to)) message.To.Add(to);
        //			}
        //			if (message.To.Count == 0) return;

        //			message.Subject = subject ?? string.Empty;
        //			message.Body = htmlBody ?? string.Empty;
        //			message.IsBodyHtml = true;

        //			using (var client = new SmtpClient(Config.SmtpHost, Config.SmtpPort))
        //			{
        //				client.EnableSsl = Config.SmtpEnableSsl;
        //				if (!string.IsNullOrWhiteSpace(Config.SmtpUser))
        //				{
        //					client.Credentials = new NetworkCredential(Config.SmtpUser, Config.SmtpPass);
        //				}
        //				else
        //				{
        //					client.UseDefaultCredentials = true;
        //				}

        //				await client.SendMailAsync(message);
        //			}
        //		}
        //	}
        //	catch
        //	{
        //	}
        //}


        public async Task SendAsync(string toEmail, string subject, string body, bool isHtml = true, string cc = "")
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtp.DisplayName, _smtp.Mail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                if (!string.IsNullOrWhiteSpace(cc))
                {
                    foreach (var mailcc in cc.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (MailboxAddress.TryParse(mailcc.Trim(), out var addr))
                        {
                            email.Cc.Add(addr);
                        }
                    }

                    email.Cc.Add(MailboxAddress.Parse(toEmail));
                }
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                                    <html>
                                    <head>
                                    <meta charset='UTF-8'>
                                    </head>
                                    <body >
                                        <div style='font-family: ""Times New Roman"", Times, serif; font-size:14px;'>{body}</div>
                                    </body>
                                    </html>";


                email.Body = builder.ToMessageBody();
                //email.Body = new TextPart(isHtml ? "html" : "plain")
                //{
                //    Text = body
                //};

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_smtp.Mail, _smtp.Password);
                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task SendAsyncHr(string toEmail, string subject, string body, bool isHtml = true, string cc = "")
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_smtpHr.DisplayName, _smtpHr.Mail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                if (!string.IsNullOrWhiteSpace(cc))
                {
                    foreach (var mailcc in cc.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (MailboxAddress.TryParse(mailcc.Trim(), out var addr))
                        {
                            email.Cc.Add(addr);
                        }
                    }

                    email.Cc.Add(MailboxAddress.Parse(toEmail));
                }
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                                    <html>
                                    <head>
                                    <meta charset='UTF-8'>
                                    </head>
                                    <body >
                                        <div style='font-family: ""Times New Roman"", Times, serif; font-size:14px;'>{body}</div>
                                    </body>
                                    </html>";


                email.Body = builder.ToMessageBody();
                //email.Body = new TextPart(isHtml ? "html" : "plain")
                //{
                //    Text = body
                //};

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtpHr.Host, _smtpHr.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_smtpHr.Mail, _smtpHr.Password);
                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}



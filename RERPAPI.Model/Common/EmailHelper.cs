using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
	public static class EmailHelper
	{
		public static async Task SendAsync(string[] toEmails, string subject, string htmlBody)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Config.SmtpHost) || string.IsNullOrWhiteSpace(Config.SmtpFrom))
				{
					// Chưa cấu hình SMTP => bỏ qua gửi mail
					return;
				}

				using (var message = new MailMessage())
				{
					message.From = new MailAddress(Config.SmtpFrom, Config.SmtpFromDisplay ?? Config.SmtpFrom);
					foreach (var to in toEmails ?? Array.Empty<string>())
					{
						if (!string.IsNullOrWhiteSpace(to)) message.To.Add(to);
					}
					if (message.To.Count == 0) return;

					message.Subject = subject ?? string.Empty;
					message.Body = htmlBody ?? string.Empty;
					message.IsBodyHtml = true;

					using (var client = new SmtpClient(Config.SmtpHost, Config.SmtpPort))
					{
						client.EnableSsl = Config.SmtpEnableSsl;
						if (!string.IsNullOrWhiteSpace(Config.SmtpUser))
						{
							client.Credentials = new NetworkCredential(Config.SmtpUser, Config.SmtpPass);
						}
						else
						{
							client.UseDefaultCredentials = true;
						}

						await client.SendMailAsync(message);
					}
				}
			}
			catch
			{
			}
		}
	}
}



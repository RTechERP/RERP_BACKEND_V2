namespace RERPAPI.Model.Common
{
    public static class Config
    {
        private static int _isPublish = 0;
        //public static string Path()
        //{
        //    string path = @"D:\\LeTheAnh\\RTC";
        //    return path;
        //}
        public static string ConnectionString { get; set; } = @"";

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        string connectionString = @"Server=DESKTOP-GQKB5KK\SQLEXPRESS;database=RTC_Test;User Id = sa; Password=1;TrustServerCertificate=True";
        //        if (_isPublish == 1) connectionString = @"";
        //        return connectionString;
        //    }
        //}

        public static string SmtpHost => Environment.GetEnvironmentVariable("SMTP_HOST") ?? "";
        public static int SmtpPort => int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var p) ? p : 25;
        public static bool SmtpEnableSsl => (Environment.GetEnvironmentVariable("SMTP_SSL") ?? "false").ToLower() == "true";
        public static string SmtpUser => Environment.GetEnvironmentVariable("SMTP_USER") ?? "";
        public static string SmtpPass => Environment.GetEnvironmentVariable("SMTP_PASS") ?? "";
        public static string SmtpFrom => Environment.GetEnvironmentVariable("SMTP_FROM") ?? "";
        public static string? SmtpFromDisplay => Environment.GetEnvironmentVariable("SMTP_FROM_DISPLAY");

        // Danh sách email nhận thông báo đăng ký thăm nhà máy, phân tách bởi dấu phẩy
        public static string[] VisitFactoryNotifyEmails
        {
            get
            {
                var raw = Environment.GetEnvironmentVariable("VISIT_FACTORY_NOTIFY") ?? string.Empty;
                return raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
        }
    }
}

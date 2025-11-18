namespace RERPAPI.Model.Common
{
    public static class Config
    {
        private static int _isPublish = 0;
        public static string Path()
        {
            string path = @"D:\\LeTheAnh\\RTC";
            return path;
        }
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
                string connectionString = @"Data Source=DESKTOP-ME2R5GM;Initial Catalog=RTC;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                //string connectionString = @"Data Source=192.168.1.2,9000;Initial Catalog=RTC;User ID=sa;Password=1;Trust Server Certificate=True";

                if (_isPublish == 1) connectionString = @"";
                return connectionString;
            }
        }
    }
}

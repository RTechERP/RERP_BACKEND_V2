namespace RERPAPI.Model.Common
{
    public class SmtpSettingsHr
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }
}
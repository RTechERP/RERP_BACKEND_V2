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
        public static string _connectionString { get; set; } = "";

        public static string ConnectionString
        {
            get
            {
                //_connectionString = @"Server=LAPTOP-PFOO9T76\LENOVO;database=RTC;User Id = sa; Password=1;TrustServerCertificate=True";
                string connectionString = @"Server=192.168.1.2,9000;database=RTC;User Id = sa; Password=1;TrustServerCertificate=True";
                if (_isPublish == 1) _connectionString = @"";
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }


    }
}

namespace RERPAPI.Model.Param
{
    /// <summary>Subject/Body đã được render sẵn ở Frontend (thay placeholder), Backend chỉ gửi và đánh dấu IsSend.</summary>
    public class SalaryIncreaseSendMailParam
    {
        public int DetailID { get; set; }
        public string EmailTo { get; set; } = string.Empty;
        public string EmailCC { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class SalaryIncreaseSendMailResultItem
    {
        public int DetailID { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

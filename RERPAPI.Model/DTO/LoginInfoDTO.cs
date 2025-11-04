namespace RERPAPI.Model.DTO
{
    public class LoginInfoDTO
    {
        public string LoginName { get; set; }
        public string PasswordHash { get; set; }
        public bool Status { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public int TeamID { get; set; }
        public int UserID { get; set; }
    }
}

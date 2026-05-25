namespace RERPAPI.Model.Param
{
    public class ChangePasswordParam
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

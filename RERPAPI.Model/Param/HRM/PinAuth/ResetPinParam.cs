namespace RERPAPI.Model.Param.HRM.PinAuth
{
    public class ResetPinParam
    {
        public string? Token { get; set; }
        public string? NewPin { get; set; }
        public string? ConfirmPin { get; set; }
    }
}

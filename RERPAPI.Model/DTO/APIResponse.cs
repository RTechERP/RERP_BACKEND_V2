namespace RERPAPI.Model.DTO
{
    public class APIResponse
    {
        public int status { get; set; }
        public string message { get; set; } = string.Empty;
        public object data { get; set; } = new object();
        public string error { get; set; } = string.Empty;
    }
}
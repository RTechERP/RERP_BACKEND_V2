namespace RERPAPI.Model.Param.HRM
{
    public class EmployeeErrorRequestParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string? KeyWord { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
    }
}
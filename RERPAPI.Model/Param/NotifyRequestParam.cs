namespace RERPAPI.Model.Param
{
    public class NotifyRequestParam
    {
        public string title { get; set; }
        public string text { get; set; }
        public int employeeID { get; set; }
        public int? departmentID { get; set; }
    }
}
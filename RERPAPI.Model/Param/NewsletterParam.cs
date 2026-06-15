namespace RERPAPI.Model.Param
{
    public class NewsletterParam
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Keyword { get; set; } = "";
        public int TypeId { get; set; } = 0;
        public int IsPublish { get; set; } = -1;
    }
}
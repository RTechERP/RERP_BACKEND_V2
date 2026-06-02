using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class NewsletterDTO
    {
        public Newsletter? Newsletter { get; set; }

        public List<NewsletterFile>? NewsletterFiles { get; set; }
    }
}
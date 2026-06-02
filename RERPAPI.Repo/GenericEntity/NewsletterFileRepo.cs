using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class NewsletterFileRepo : GenericRepo<NewsletterFile>
    {
        public NewsletterFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
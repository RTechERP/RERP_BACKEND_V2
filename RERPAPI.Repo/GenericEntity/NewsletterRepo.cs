using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class NewsletterRepo : GenericRepo<Newsletter>
    {
        public NewsletterRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BusinessFieldLinkRepo : GenericRepo<BusinessFieldLink>
    {
        public BusinessFieldLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
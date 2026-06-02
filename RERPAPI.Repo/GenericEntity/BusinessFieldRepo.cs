using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BusinessFieldRepo : GenericRepo<BusinessField>
    {
        public BusinessFieldRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
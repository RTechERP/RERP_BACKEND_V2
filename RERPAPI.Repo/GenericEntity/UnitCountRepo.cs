using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UnitCountRepo : GenericRepo<UnitCount>
    {
        public UnitCountRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool ValidateCode(UnitCount u)
        {
            var unitexxist = GetAll(x => x.UnitCode == u.UnitCode && x.UnitName == u.UnitName && x.IsDeleted == false && x.ID != u.ID);
            if (unitexxist.Any())
            {
                return false;
            }
            return true;
        }
    }
}
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class UnitCountRepo : GenericRepo<UnitCount>
    {
        public bool ValidateCode(UnitCount u)
        {
            var unitexxist = GetAll(x => x.UnitCode == u.UnitCode && x.IsDeleted == false && x.ID != u.ID);
            if (unitexxist.Any())
            {
                return false;
            }
            return true;
        }
    }
}

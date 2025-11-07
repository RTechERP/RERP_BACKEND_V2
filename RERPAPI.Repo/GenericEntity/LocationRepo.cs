using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class LocationRepo : GenericRepo<Location>
    {
        public bool CheckLocationCodeExists(string locationCode, int? id = null)
        {
            var query = GetAll(x => x.LocationCode.ToUpper() == locationCode.ToUpper() && x.IsDeleted == false);
            if (id.HasValue)
            {
                query = query.Where(x => x.ID != id.Value).ToList();
            }
            return query.Any();
        }
    }
}

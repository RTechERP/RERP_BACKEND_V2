using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductLocationRepo : GenericRepo<ProductLocation>
    {
        public ProductLocationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool CheckLocationCodeExists(string locationCode, int? excludeId = null)
        {
            try
            {
                if (excludeId.HasValue)
                {
                    return GetAll(x => x.LocationCode == locationCode && x.IsDeleted != true && x.ID != excludeId.Value).Any();
                }
                else
                {
                    return GetAll(x => x.LocationCode == locationCode && x.IsDeleted != true).Any();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo mã: " + ex.Message, ex);
            }
        }
    }
}

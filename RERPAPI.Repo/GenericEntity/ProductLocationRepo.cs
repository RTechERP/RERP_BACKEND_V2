using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductLocationRepo : GenericRepo<ProductLocation>
    {
       private readonly ProductRTCRepo _productRtcRepo;
        public ProductLocationRepo(CurrentUser currentUser, ProductRTCRepo productRtcRepo) : base(currentUser)
        {
            _productRtcRepo = productRtcRepo;
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


        public bool Validate(ProductLocation location, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(location.LocationCode))
            {
                message = "Cần nhập đầy đủ thông tin vào các ô còn trống!";
                return false;
            }
            var existLocation = GetAll(x => x.LocationType == location.LocationType && x.WarehouseID == location.WarehouseID && x.ID != location.ID && x.LocationCode.Trim() == location.LocationCode.Trim() && x.IsDeleted == false).FirstOrDefault();
            if (existLocation != null && existLocation.ID > 0)
            {
                message = $"Mã [{location.LocationCode}] đã tồn tại, vui lòng kiểm tra lại";
                return false;
            }
            return true;
        }
        public int GetSTT(int warehouseID)
        {
            var listLocations = GetAll(x => x.WarehouseID == warehouseID);
            int stt = listLocations.Max(x => x.STT ?? 0) + 1;
            return stt;
        }
        public bool CheckLocationInUse(int id)
        {
            var exist = _productRtcRepo.GetAll(x => x.ProductLocationID == id);
            return exist.Any();
        }
    }
}

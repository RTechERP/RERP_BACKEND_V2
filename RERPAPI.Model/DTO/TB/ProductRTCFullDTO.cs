using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.TB
{
    public class ProductRTCFullDTO
    {
        public List<ProductRTC>? productRTCs { get; set; }
        public ProductGroupRTC? productGroupRTC { get; set; }
    }
}

using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class FixedProductValidationResultDTO
    {
        public bool HasMismatch { get; set; }
        public List<string> MismatchedFields { get; set; } = new List<string>();
        public ProductSale FixedProductData { get; set; }
    }
}
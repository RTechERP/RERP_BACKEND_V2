using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductGroupRTCRepo : GenericRepo<ProductGroupRTC>
    {
        public ProductGroupRTCRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string GenerateCode(int warehouseType)
        {
            var dt = GetAll(x => x.WarehouseType == warehouseType && x.IsDeleted == false);

            string prefix = "A";

            if (dt == null || !dt.Any())
                return $"{prefix}01";

            var codes = dt
                .Select(x => x.ProductGroupNo)
                .Where(code => !string.IsNullOrEmpty(code) && code.StartsWith(prefix))
                .ToList();

            int maxNumber = 0;

            foreach (var code in codes)
            {
                string numPart = code.Substring(prefix.Length);

                if (int.TryParse(numPart, out int num))
                {
                    if (num > maxNumber)
                        maxNumber = num;
                }
            }

            int nextNumber = maxNumber + 1;

            return $"{prefix}{nextNumber:D2}";
        }

    }
}

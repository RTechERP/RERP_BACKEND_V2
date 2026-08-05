using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductSaleRepo : GenericRepo<ProductSale>
    {
        private LocationRepo _locationRepo;
        private CurrentUser _currentUser;
        private ProductGroupRepo _productgroupRepo;

        public ProductSaleRepo(CurrentUser currentUser, LocationRepo locationRepo, ProductGroupRepo productgroupRepo) : base(currentUser)
        {
            _locationRepo = locationRepo;
            _currentUser = currentUser;
            _productgroupRepo = productgroupRepo;
        }

        public async Task<bool> SetLocationList(SetLocationRequestDTO req)
        {
            int count = 0;
            var loc = _locationRepo.GetByID(req.LocationID);
            if (loc.ID <= 0) return false;
            foreach (var id in req.LstIDs)
            {
                var productSale = GetByID(id);

                if (productSale.ID > 0)
                {
                    var param = new
                    {
                        WarehouseID = req.WarehouseID,
                        ProductGroupID = productSale.ProductGroupID,
                    };

                    List<spGetProductGroupWarehouseResultDTO> listEmployee = await SqlDapper<spGetProductGroupWarehouseResultDTO>.ProcedureToListTAsync("spGetProductGroupWarehouse", param);
                    if (listEmployee.Count <= 0 && !_currentUser.IsAdmin) continue;
                    bool hasInvalidUser = listEmployee.Any(item => item.UserID != _currentUser.ID && !_currentUser.IsAdmin);
                    if (hasInvalidUser) continue;
                    productSale.LocationID = req.LocationID;
                    productSale.AddressBox = loc.LocationName;
                    Update(productSale);
                    count++;
                }
            }
            return count > 0;
        }

        public bool SetLocation(int locationID, int productsaleID)
        {
            var productSale = GetByID(productsaleID);
            var loc = _locationRepo.GetByID(locationID);
            if (loc.ID <= 0 || productSale.ID <= 0) return false;
            productSale.LocationID = locationID;
            productSale.AddressBox = loc.LocationName;
            Update(productSale);
            return true;
        }

        //public string GetMaxProductNewCode(int productGroupId, string productGroupCode)
        //{


        //    return table.Where(x => x.ProductGroupID == productGroupId &&
        //                            x.ProductNewCode != null &&
        //                            x.ProductNewCode != "" &&
        //                            x.ProductNewCode.StartsWith(productGroupCode))
        //                .Select(x => x.ProductNewCode)
        //                .OrderByDescending(x => x.Length)
        //                .ThenByDescending(x => x)
        //                .FirstOrDefault();
        //}

        public string GetMaxProductNewCode(int productGroupId, string productGroupCode)
        {
            var currentGroup = _productgroupRepo.GetByID(productGroupId);
            if (currentGroup == null)
                return string.Empty;

            string groupCode = currentGroup.ProductGroupID.Trim();

            var groupIds = _productgroupRepo
                .GetAll(x => x.ProductGroupID == groupCode)
                .Select(x => x.ID)
                .ToList();

            return table
                    .Where(x => x.ProductGroupID.HasValue
                                && groupIds.Contains(x.ProductGroupID.Value)
                                && !string.IsNullOrWhiteSpace(x.ProductNewCode)
                                && x.ProductNewCode.StartsWith(groupCode))
                    .AsEnumerable()   
                    .Select(x => new
                    {
                        Code = x.ProductNewCode,
                        Number = int.TryParse(
                            x.ProductNewCode.Substring(groupCode.Length),
                            out int num)
                            ? num
                            : 0
                    })
                    .OrderByDescending(x => x.Number)
                    .Select(x => x.Code)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Tìm sản phẩm theo Mã SP trong nhóm đích; nếu chưa có thì tạo mới
        /// (dùng cho chuyển kho nội bộ giữa 2 loại kho/nhóm SP).
        /// Tên/Hãng/ĐVT trùng khớp đã được đảm bảo ở bước validate trước khi lưu.
        /// Mã nội bộ (ProductNewCode) của sản phẩm mới được sinh qua <paramref name="generateProductNewCode"/>
        /// (dùng chung logic với ProjectPartlistPurchaseRequestRepo.GenerateProductNewCode).
        /// </summary>
        public async Task<ProductSale> GetOrCreateForGroupAsync(ProductSale source, int destGroupId, string createdBy, Func<int, string> generateProductNewCode)
        {
            var match = GetAll(x => x.ProductGroupID == destGroupId && x.IsDeleted != true
                    && (x.ProductCode ?? "").Trim().ToLower() == (source.ProductCode ?? "").Trim().ToLower())
                .FirstOrDefault();
            if (match != null) return match;

            var created = new ProductSale
            {
                ProductCode = source.ProductCode,
                ProductName = source.ProductName,
                Maker = source.Maker,
                Unit = source.Unit,
                FirmID = source.FirmID,
                UnitCountID = source.UnitCountID,
                ProductGroupID = destGroupId,
                ProductNewCode = generateProductNewCode(destGroupId),
                Import = 0,
                Export = 0,
                NumberInStoreDauky = 0,
                NumberInStoreCuoiKy = 0,
                IsFix = source.IsFix,
                IsStandardized = source.IsStandardized,
                IsApproved = source.IsApproved,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CreatedBy = createdBy,
            };
            await CreateAsync(created);
            return created;
        }
    }
}
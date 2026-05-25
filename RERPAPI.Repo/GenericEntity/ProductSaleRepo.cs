using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductSaleRepo : GenericRepo<ProductSale>
    {
        LocationRepo _locationRepo;
        CurrentUser _currentUser;
        public ProductSaleRepo(CurrentUser currentUser, LocationRepo locationRepo) : base(currentUser)
        {
            _locationRepo = locationRepo;
            _currentUser = currentUser;
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
    }
}

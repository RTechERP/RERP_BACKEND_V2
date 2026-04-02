using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductSaleRepo : GenericRepo<ProductSale>
    {
        LocationRepo _locationRepo;
        public ProductSaleRepo(CurrentUser currentUser, LocationRepo locationRepo) : base(currentUser)
        {
            _locationRepo = locationRepo;
        }
        public bool SetLocationList(int locationID, List<int> lstIDs)
        {
            int count = 0;
            var loc = _locationRepo.GetByID(locationID);
            if (loc.ID <= 0) return false;
            foreach (var id in lstIDs)
            {
                var productSale = GetByID(id);
                if (productSale.ID > 0)
                {
                    productSale.LocationID = locationID;
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

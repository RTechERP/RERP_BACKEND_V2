using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class HistoryProductRTCRepo : GenericRepo<HistoryProductRTC>
    {
        const int WAREHOUSE_ID = 1;
        private readonly ProductRTCQRCodeRepo _productRTCQRCodeRepo;
        CurrentUser _currentUser;
        public HistoryProductRTCRepo(CurrentUser currentUser, ProductRTCQRCodeRepo productRTCQRCodeRepo) : base(currentUser)
        {
            _productRTCQRCodeRepo = productRTCQRCodeRepo;
            _currentUser = currentUser;
        }
        public async Task SaveDataAsync(ModulaLocationDTO.SerialNumberModulaLocation item)
        {
            string productQRCode = item.SerialNumber ?? "";
            HistoryProductRTC historyProductRTC = GetByID(item.HistoryProductRTCID);
            //if (historyProductRTC.ID <= 0 && item.Status == 7)
            if (historyProductRTC.ID <= 0)
            {
                ProductRTCQRCode qrCode = _productRTCQRCodeRepo.GetAll(x => x.ProductQRCode == productQRCode.Trim()).FirstOrDefault() ?? new ProductRTCQRCode();
                historyProductRTC = new HistoryProductRTC()
                {
                    ProductRTCID = item.ProductRTCID, //Nhập từ web
                    DateBorrow = DateTime.Now,
                    DateReturnExpected = item.DateReturnExpected, //Nhập từ web
                    PeopleID = item.PeopleID,
                    Project = item.Project,
                    Note = item.Note,
                    Status = item.Status,
                    NumberBorrow = item.Quantity,
                    AdminConfirm = false,
                    BillExportTechnicalID = 0,
                    ProductRTCQRCodeID = qrCode.ID,
                    WarehouseID = WAREHOUSE_ID,
                    ProductRTCQRCode = item.SerialNumber,
                    ProductLocationID = 0,
                    CreatedBy = item.CreatedBy,
                    UpdatedBy = item.CreatedBy,
                    IsDelete = false,
                };

                await CreateAsync(historyProductRTC);
            }
            else if (historyProductRTC.ID > 0 && item.Status == 4)
            {
                if (item.PeopleID == historyProductRTC.PeopleID) //Nếu là người mượn thì mới được trả
                {
                    historyProductRTC.DateReturn = DateTime.Now;
                    historyProductRTC.Status = item.Status;

                    historyProductRTC.UpdatedBy = item.CreatedBy;
                    historyProductRTC.UpdatedDate = DateTime.Now;

                    await UpdateAsync(historyProductRTC);
                }


            }

        }

    }
}

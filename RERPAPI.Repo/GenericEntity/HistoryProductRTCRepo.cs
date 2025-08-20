using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Technical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class HistoryProductRTCRepo : GenericRepo<HistoryProductRTC>
    {
        const int WAREHOUSE_ID = 1;
        ProductRTCQRCodeRepo _qrCodeRepo = new ProductRTCQRCodeRepo();

        public async Task SaveDataAsync(ModulaLocationDTO.SerialNumberModulaLocation item)
        {
            string productQRCode = item.SerialNumber ?? "";
            HistoryProductRTC historyProductRTC = GetByID(item.HistoryProductRTCID);
            if (historyProductRTC.ID <= 0 && item.Status == 7)
            {
                ProductRTCQRCode qrCode = _qrCodeRepo.GetAll().FirstOrDefault(x => x.ProductQRCode == productQRCode.Trim()) ?? new ProductRTCQRCode();
                historyProductRTC = new HistoryProductRTC()
                {
                    ProductRTCID = item.ProductRTCID, //Nhập từ web
                    DateBorrow = DateTime.Now,
                    DateReturnExpected = item.DateReturnExpected, //Nhập từ web
                    PeopleID = item.PeopleID,
                    Project = item.Project,//Nhập từ web
                    //DateReturn = null,
                    Note = item.Note,//Nhập từ web
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

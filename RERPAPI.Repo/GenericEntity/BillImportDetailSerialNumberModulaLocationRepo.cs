using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportDetailSerialNumberModulaLocationRepo : GenericRepo<BillImportDetailSerialNumberModulaLocation>
    {
        BillImportTechDetailSerialRepo _importDetailSerialNumberRepo = new BillImportTechDetailSerialRepo();
        ProductRTCQRCodeRepo _qrCodeRepo = new ProductRTCQRCodeRepo();


        const int WARE_HOUSE_ID = 1;
        public async Task<string> SaveDataAsync(ModulaLocationDTO.SerialNumberModulaLocation item, int index)
        {
            //if (string.IsNullOrWhiteSpace(item.SerialNumber)) continue;

            string message = "";

            BillImportTechDetailSerial serialNumber = _importDetailSerialNumberRepo.GetAll()
                                                                                  .FirstOrDefault(x => x.SerialNumber == item.SerialNumber && x.BillImportTechDetailID > 0)
                                                                                  ?? new BillImportTechDetailSerial();
            var qrCodes = _qrCodeRepo.GetAll().Where(x => x.ProductQRCode == item.SerialNumber.Trim()).ToList();
            if (qrCodes.Count() <= 0)
            {
                ProductRTCQRCode productRTCQRCode = new ProductRTCQRCode()
                {
                    Status = 1,
                    ProductRTCID = item.ProductRTCID,
                    ProductQRCode = item.SerialNumber.Trim(),
                    Serial = "",
                    SerialNumber = "",
                    PartNumber = "",
                    CreatedBy = item.CreatedBy,
                    UpdatedBy = item.CreatedBy,
                    WarehouseID = WARE_HOUSE_ID
                };

                await _qrCodeRepo.CreateAsync(productRTCQRCode);
            }
            else
            {
                qrCodes = qrCodes.Where(x => x.ProductRTCID != item.ProductRTCID).ToList();
                if (qrCodes.Count() > 0)
                {
                    message = $"QR Code [{item.SerialNumber}] đã tồn tại ở sẩn phẩm khác!";
                }
            }
            if (serialNumber.ID <= 0)
            {
                serialNumber.STT = index + 1;
                serialNumber.BillImportTechDetailID = item.BillImportDetailTechnicalID;
                serialNumber.SerialNumber = item.SerialNumber.Trim();
                serialNumber.WarehouseID = 1;

                _importDetailSerialNumberRepo.Create(serialNumber);
            }

            BillImportDetailSerialNumberModulaLocation import = new BillImportDetailSerialNumberModulaLocation()
            {
                //BillImportTechDetailSerialID = serialNumber.ID,
                ModulaLocationDetailID = item.ModulaLocationDetailID,
                Quantity = item.Quantity,
                IsDeleted = false,
                CreatedBy = item.CreatedBy,
                UpdatedBy = item.CreatedBy,

                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                BillImportDetailSerialNumberID = 0
            };

            await CreateAsync(import);

            return message;
        }
    }
}

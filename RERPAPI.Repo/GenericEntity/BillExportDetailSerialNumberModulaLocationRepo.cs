﻿using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillExportDetailSerialNumberModulaLocationRepo:GenericRepo<BillExportDetailSerialNumberModulaLocation>
    {
        BillExportTechDetailSerialRepo _exportDetailSerialNumberRepo = new BillExportTechDetailSerialRepo();
        public async Task SaveDataAsync(ModulaLocationDTO.SerialNumberModulaLocation item, int index)
        {
            BillExportTechDetailSerial serialNumber = _exportDetailSerialNumberRepo.GetAll().FirstOrDefault(x => x.SerialNumber == item.SerialNumber) ?? new BillExportTechDetailSerial();
            if (serialNumber.ID <= 0)
            {
                serialNumber.STT = index + 1;
                serialNumber.BillExportTechDetailID = item.BillExportDetailID;
                serialNumber.SerialNumber = item.SerialNumber.Trim();
                serialNumber.WarehouseID = 1;

                _exportDetailSerialNumberRepo.Create(serialNumber);
            }

            BillExportDetailSerialNumberModulaLocation export = new BillExportDetailSerialNumberModulaLocation()
            {
                BillExportTechDetailSerialID = serialNumber.ID,
                ModulaLocationDetailID = item.ModulaLocationDetailID,
                Quantity = item.Quantity,
                IsDeleted = false,
                CreatedBy = item.CreatedBy,
                UpdatedBy = item.CreatedBy,

                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,

                BillExportDetailSerialNumberID = 0
            };

            await CreateAsync(export);
        }
    }
}

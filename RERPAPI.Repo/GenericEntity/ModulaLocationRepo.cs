using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ModulaLocationRepo : GenericRepo<ModulaLocation>
    {

        BillImportTechDetailSerialRepo importDetailSerialNumberRepo = new BillImportTechDetailSerialRepo();

        public APIResponse CheckValidate(List<ModulaLocationDTO.SerialNumberModulaLocation> serialNumberModulaLocations)
        {

            APIResponse response = new APIResponse() { status = 1 };

            for (int i = 0; i < serialNumberModulaLocations.Count; i++)
            {
                var item = serialNumberModulaLocations[i];
                if (string.IsNullOrWhiteSpace(item.SerialNumber)) continue;

                if (item.BillImportDetailTechnicalID > 0) //Nếu là nhập kho
                {
                    //check trong request truyền lên
                    var serialNumberRequest = serialNumberModulaLocations.Where(x => x.SerialNumber == item.SerialNumber).ToList();
                    if (serialNumberRequest.Count() > 1)
                    {
                        response.status = 0;
                        response.message = $"SerialNumber [{item.SerialNumber}] đã tồn tại trong danh sách!";

                        break;
                    }

                    //check trong database
                    var serialNumbers = importDetailSerialNumberRepo.GetAll().Where(x => x.SerialNumber == item.SerialNumber && x.BillImportTechDetailID > 0).ToList();
                    if (serialNumbers.Count() > 0)
                    {
                        response.status = 0;
                        response.message = $"SerialNumber [{item.SerialNumber}] đã được nhập ở vị trí khác!";

                        break;
                    }
                }
            }


            return response;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace RERPAPI.Repo.GenericEntity
{
    public class HRHiringRequestRepo : GenericRepo<HRHiringRequest>
    {
        
        public int GetSTT()
        {
            try
            {
                var hrHirings = GetAll(x => x.IsDeleted == true);
                int stt = hrHirings.Count() <= 0 ? 1 : Convert.ToInt32(hrHirings.Max(x => x.STT)) + 1;
                return stt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public HRHiringRequest GetRequestCode()
        {
            try
            {
                HRHiringRequest request = new HRHiringRequest();
                string requestCode = "";
                string requestCodePrefex = "YCTD_";
                string dateRequest = DateTime.Now.ToString("yyyyMMdd_");

                var hrHirings = GetAll(x => x.IsDeleted == true);
                int stt = hrHirings.Count() <= 0 ? 1 : Convert.ToInt32(hrHirings.Max(x => x.STT)) + 1;

                var hrHiringCodes = hrHirings.Where(x => x.DateRequest.Value.Year == DateTime.Now.Year &&
                                                        x.DateRequest.Value.Month == DateTime.Now.Month &&
                                                        x.DateRequest.Value.Date == DateTime.Now.Date)
                                            .Select(x => new
                                            {
                                                ID = x.ID,
                                                Code = x.HiringRequestCode,
                                                STT = string.IsNullOrWhiteSpace(x.HiringRequestCode) ? 0 : Convert.ToInt32(x.HiringRequestCode.Substring(x.HiringRequestCode.Length - 3)),
                                            }).ToList();

                string numberCodeText = "000";
                int numberCode = hrHiringCodes.Count <= 0 ? 0 : hrHiringCodes.Max(x => x.STT);
                numberCodeText = (++numberCode).ToString();
                while (numberCodeText.Length < 3)
                {
                    numberCodeText = "0" + numberCodeText;
                }

                requestCode = $"{requestCodePrefex}{dateRequest}{numberCodeText}";

                request.STT = stt;
                request.HiringRequestCode = requestCode;
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RERPAPI.Model.DTO.ESL;
using RERPAPI.Repo.GenericEntity.ESL;

namespace RERPAPI.Controllers.ESL
{
    public interface IESLBindService
    {
        Task<ESLBindResponse> UpdateProductAsync(object requestPayload);
        Task<List<EslDevice>> GetEslDevicesAsync();
    }

    public class ESLBindService : IESLBindService
    {
        private readonly ESLConfigRepo _configRepo;
        private readonly HttpClient _httpClient;

        public ESLBindService(ESLConfigRepo configRepo, HttpClient httpClient)
        {
            _configRepo = configRepo;
            _httpClient = httpClient;
        }

        public async Task<List<EslDevice>> GetEslDevicesAsync()
        {
            try
            {
                var configs = _configRepo.GetAll();
                string baseUrl = configs.Find(x => x.ConfigKey == "ESL_API_BASE_URL")?.ConfigValue ?? "https://la-woesl.cloud/";
                string store_code = configs.Find(x => x.ConfigKey == "ESL_STORE_CODE")?.ConfigValue ?? "rtc01";
                string esl_sign = configs.Find(x => x.ConfigKey == "ESL_SIGN")?.ConfigValue ?? "80805d794841f1b4";
                string url = $"{baseUrl.TrimEnd('/')}/api/default/esl/query?f1=1&f2=200&store_code={store_code}&is_base64=0&sign={esl_sign}";
                var json = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<List<EslDevice>>(json) ?? new List<EslDevice>();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<ESLBindResponse> UpdateProductAsync(object requestPayload)
        {
            try
            {
                var configs = _configRepo.GetAll();
                string baseUrl = configs.Find(x => x.ConfigKey == "ESL_API_BASE_URL")?.ConfigValue ?? "https://la-woesl.cloud/";

                string jsonContent = JsonSerializer.Serialize(requestPayload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                string url = $"{baseUrl.TrimEnd('/')}/api/default/product/create";

                var response = await _httpClient.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<ESLBindResponse>(responseString);
                        return result;
                    }
                    catch
                    {
                        return new ESLBindResponse { code = (int)response.StatusCode, message = responseString };
                    }
                }
                else
                {
                    return new ESLBindResponse { code = (int)response.StatusCode, message = $"HTTP Error: {responseString}" };
                }
            }
            catch (Exception ex)
            {
                return new ESLBindResponse { code = 500, message = ex.Message };
            }
        }






    }
}
